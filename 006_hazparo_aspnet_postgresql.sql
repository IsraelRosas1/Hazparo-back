-- ============================================================================
-- HAZPARO — PostgreSQL schema for an ASP.NET Core API
-- PostgreSQL 15+ with PostGIS | Version 1.0 | 2026-10-30
-- ============================================================================
--
-- PURPOSE
--   Fresh-database schema for Hazparo's ASP.NET Core backend. This replaces
--   the Supabase-specific model in schema_glamapp_hazparo.sql and is NOT
--   compatible with supabase/sql/* or 005_hazparo_marketplace_backend.sql.
--
-- IMPORTANT
--   * Run this once against an EMPTY PostgreSQL database owned by the
--     migration user. Do not run it in a database containing either older
--     Hazparo schema.
--   * The ASP.NET API is the only component that connects to this database.
--     Mobile clients must never receive database credentials.
--   * PasswordHash values are created and verified by ASP.NET Identity (or an
--     equivalent Argon2id implementation) in the API. Never hash passwords
--     in the mobile app or store a plaintext password.
--   * Files live in object storage (S3, Azure Blob, Cloudflare R2, etc.). This
--     database stores an object key, never a public/signed URL.
--   * Authorization belongs in ASP.NET endpoint/service policies. There are
--     deliberately no Supabase Auth, RLS, storage, or realtime definitions.
--
-- Suggested ASP.NET packages:
--   Npgsql.EntityFrameworkCore.PostgreSQL
--   Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite
--   Microsoft.AspNetCore.Identity (optional, recommended)
-- ============================================================================

BEGIN;

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS postgis;
CREATE EXTENSION IF NOT EXISTS citext;

-- ---------------------------------------------------------------------------
-- Enumerations. Register these with Npgsql when using EF Core, or map their
-- values as strings in application code.
-- ---------------------------------------------------------------------------
DO $$ BEGIN
  CREATE TYPE user_role AS ENUM ('client', 'professional', 'admin');
EXCEPTION WHEN duplicate_object THEN NULL;
END $$;

DO $$ BEGIN
  CREATE TYPE account_status AS ENUM ('pending', 'active', 'suspended', 'deleted');
EXCEPTION WHEN duplicate_object THEN NULL;
END $$;

DO $$ BEGIN
  CREATE TYPE verification_status AS ENUM ('not_submitted', 'pending', 'verified', 'rejected');
EXCEPTION WHEN duplicate_object THEN NULL;
END $$;

DO $$ BEGIN
  CREATE TYPE service_request_status AS ENUM (
    'draft', 'open', 'quoted', 'assigned', 'accepted', 'en_route',
    'in_progress', 'completed', 'cancelled', 'expired'
  );
EXCEPTION WHEN duplicate_object THEN NULL;
END $$;

DO $$ BEGIN
  CREATE TYPE quote_status AS ENUM ('sent', 'accepted', 'declined', 'withdrawn', 'expired');
EXCEPTION WHEN duplicate_object THEN NULL;
END $$;

DO $$ BEGIN
  CREATE TYPE subscription_status AS ENUM ('trialing', 'active', 'past_due', 'cancelled', 'paused');
EXCEPTION WHEN duplicate_object THEN NULL;
END $$;

DO $$ BEGIN
  CREATE TYPE notification_channel AS ENUM ('push', 'in_app', 'email');
EXCEPTION WHEN duplicate_object THEN NULL;
END $$;

DO $$ BEGIN
  CREATE TYPE device_platform AS ENUM ('ios', 'android');
EXCEPTION WHEN duplicate_object THEN NULL;
END $$;

-- ---------------------------------------------------------------------------
-- Identity and authorization
-- ---------------------------------------------------------------------------
CREATE TABLE app_users (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  email                 citext NOT NULL UNIQUE,
  normalized_email      text GENERATED ALWAYS AS (upper(email::text)) STORED,
  password_hash         text NOT NULL,
  display_name          varchar(120) NOT NULL,
  phone_number          varchar(30),
  avatar_object_key     text,
  status                account_status NOT NULL DEFAULT 'pending',
  email_confirmed_at    timestamptz,
  phone_confirmed_at    timestamptz,
  last_login_at         timestamptz,
  deleted_at            timestamptz,
  created_at            timestamptz NOT NULL DEFAULT now(),
  updated_at            timestamptz NOT NULL DEFAULT now(),
  CONSTRAINT app_users_display_name_not_blank CHECK (length(btrim(display_name)) >= 2),
  CONSTRAINT app_users_phone_not_blank CHECK (phone_number IS NULL OR length(btrim(phone_number)) >= 7),
  CONSTRAINT app_users_deleted_status CHECK (
    (deleted_at IS NULL) OR status = 'deleted'
  )
);

CREATE INDEX app_users_active_idx ON app_users (status) WHERE deleted_at IS NULL;

-- A person can be both a client and a professional. It avoids two accounts
-- for one phone/e-mail while still allowing simple role-based API policies.
CREATE TABLE app_user_roles (
  user_id               uuid NOT NULL REFERENCES app_users(id) ON DELETE CASCADE,
  role                  user_role NOT NULL,
  created_at            timestamptz NOT NULL DEFAULT now(),
  PRIMARY KEY (user_id, role)
);

CREATE INDEX app_user_roles_role_idx ON app_user_roles (role, user_id);

-- Store only hashes of opaque refresh tokens. The API issues the raw token
-- once, hashes it (SHA-256 or stronger), and rotates/revokes it on use.
CREATE TABLE auth_refresh_tokens (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id               uuid NOT NULL REFERENCES app_users(id) ON DELETE CASCADE,
  token_hash            char(64) NOT NULL UNIQUE,
  expires_at            timestamptz NOT NULL,
  revoked_at            timestamptz,
  replaced_by_token_id  uuid REFERENCES auth_refresh_tokens(id) ON DELETE SET NULL,
  user_agent            text,
  ip_address            inet,
  created_at            timestamptz NOT NULL DEFAULT now(),
  CONSTRAINT auth_refresh_tokens_expiry CHECK (expires_at > created_at)
);

CREATE INDEX auth_refresh_tokens_lookup_idx
  ON auth_refresh_tokens (user_id, expires_at DESC)
  WHERE revoked_at IS NULL;

CREATE TABLE password_reset_tokens (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id               uuid NOT NULL REFERENCES app_users(id) ON DELETE CASCADE,
  token_hash            char(64) NOT NULL UNIQUE,
  expires_at            timestamptz NOT NULL,
  used_at               timestamptz,
  created_at            timestamptz NOT NULL DEFAULT now(),
  CONSTRAINT password_reset_tokens_expiry CHECK (expires_at > created_at)
);

-- ---------------------------------------------------------------------------
-- Client and professional profiles
-- ---------------------------------------------------------------------------
CREATE TABLE client_profiles (
  user_id               uuid PRIMARY KEY REFERENCES app_users(id) ON DELETE CASCADE,
  default_city          varchar(120),
  created_at            timestamptz NOT NULL DEFAULT now(),
  updated_at            timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE professional_profiles (
  user_id                     uuid PRIMARY KEY REFERENCES app_users(id) ON DELETE CASCADE,
  business_name               varchar(160),
  bio                         text,
  years_experience            smallint NOT NULL DEFAULT 0,
  hourly_rate_mxn             numeric(10,2),
  is_available                boolean NOT NULL DEFAULT false,
  accepts_emergency           boolean NOT NULL DEFAULT false,
  service_radius_km           smallint NOT NULL DEFAULT 10,
  verification                verification_status NOT NULL DEFAULT 'not_submitted',
  verified_at                 timestamptz,
  verification_notes          text,
  stripe_customer_id          text UNIQUE,
  subscription_active         boolean NOT NULL DEFAULT false,
  subscription_expires_at     timestamptz,
  rating_average              numeric(3,2) NOT NULL DEFAULT 0,
  rating_count                integer NOT NULL DEFAULT 0,
  completed_jobs_count        integer NOT NULL DEFAULT 0,
  created_at                  timestamptz NOT NULL DEFAULT now(),
  updated_at                  timestamptz NOT NULL DEFAULT now(),
  CONSTRAINT professional_profiles_years_experience CHECK (years_experience BETWEEN 0 AND 80),
  CONSTRAINT professional_profiles_hourly_rate CHECK (hourly_rate_mxn IS NULL OR hourly_rate_mxn >= 0),
  CONSTRAINT professional_profiles_radius CHECK (service_radius_km BETWEEN 1 AND 100),
  CONSTRAINT professional_profiles_rating CHECK (rating_average BETWEEN 0 AND 5),
  CONSTRAINT professional_profiles_rating_count CHECK (rating_count >= 0),
  CONSTRAINT professional_profiles_completed_jobs CHECK (completed_jobs_count >= 0)
);

CREATE INDEX professional_profiles_discovery_idx
  ON professional_profiles (is_available, subscription_active, verification)
  WHERE is_available AND subscription_active AND verification = 'verified';

-- Precise coordinates are not included in public professional profile results.
CREATE TABLE professional_locations (
  professional_id       uuid PRIMARY KEY REFERENCES professional_profiles(user_id) ON DELETE CASCADE,
  latitude              numeric(9,6) NOT NULL CHECK (latitude BETWEEN -90 AND 90),
  longitude             numeric(9,6) NOT NULL CHECK (longitude BETWEEN -180 AND 180),
  location              geography(Point, 4326) NOT NULL,
  city                  varchar(120),
  updated_at            timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX professional_locations_geo_idx ON professional_locations USING gist (location);

CREATE TABLE client_addresses (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  client_id             uuid NOT NULL REFERENCES client_profiles(user_id) ON DELETE CASCADE,
  label                 varchar(80) NOT NULL DEFAULT 'Casa',
  recipient_name        varchar(120),
  phone_number          varchar(30),
  address_line1         varchar(200) NOT NULL,
  address_line2         varchar(200),
  neighborhood          varchar(120),
  city                  varchar(120) NOT NULL,
  state                 varchar(120),
  postal_code           varchar(20),
  country_code          char(2) NOT NULL DEFAULT 'MX',
  latitude              numeric(9,6) NOT NULL CHECK (latitude BETWEEN -90 AND 90),
  longitude             numeric(9,6) NOT NULL CHECK (longitude BETWEEN -180 AND 180),
  location              geography(Point, 4326) NOT NULL,
  is_default            boolean NOT NULL DEFAULT false,
  created_at            timestamptz NOT NULL DEFAULT now(),
  updated_at            timestamptz NOT NULL DEFAULT now()
);

CREATE UNIQUE INDEX client_addresses_one_default_idx
  ON client_addresses (client_id) WHERE is_default;
CREATE INDEX client_addresses_client_idx ON client_addresses (client_id, created_at DESC);

-- ---------------------------------------------------------------------------
-- Trade catalogue and professional coverage
-- ---------------------------------------------------------------------------
CREATE TABLE service_categories (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  group_name            varchar(100) NOT NULL,
  slug                  varchar(100) NOT NULL UNIQUE,
  name                  varchar(140) NOT NULL,
  icon_key              varchar(80),
  search_aliases        text[] NOT NULL DEFAULT '{}',
  is_active             boolean NOT NULL DEFAULT true,
  sort_order            smallint NOT NULL,
  created_at            timestamptz NOT NULL DEFAULT now(),
  updated_at            timestamptz NOT NULL DEFAULT now(),
  CONSTRAINT service_categories_name_not_blank CHECK (length(btrim(name)) > 0),
  CONSTRAINT service_categories_group_not_blank CHECK (length(btrim(group_name)) > 0)
);

CREATE INDEX service_categories_visible_idx
  ON service_categories (group_name, sort_order)
  WHERE is_active;

-- The 52 current Hazparo categories shown in the mobile catalogue.
INSERT INTO service_categories (group_name, slug, name, sort_order) VALUES
  ('Casa','plomeria','Plomería',1),
  ('Casa','electricidad','Electricidad',2),
  ('Casa','cerrajeria','Cerrajería',3),
  ('Casa','destapacanos','Destapacaños',4),
  ('Casa','boilers-y-gas','Boilers y gas',5),
  ('Casa','aire-acondicionado','Aire acondicionado',6),
  ('Casa','linea-blanca','Línea blanca',7),
  ('Casa','herreria-de-seguridad','Herrería de seguridad',8),
  ('Casa','herreria-decorativa','Herrería decorativa',9),
  ('Casa','bombas-e-hidroneumaticos','Bombas e hidroneumáticos',10),
  ('Casa','impermeabilizacion','Impermeabilización',11),
  ('Casa','vidrio-y-aluminio','Vidrio y aluminio',12),
  ('Casa','tablaroca','Tablaroca',13),
  ('Casa','carpinteria-a-medida','Carpintería a medida',14),
  ('Casa','cisternas-y-tinacos','Cisternas y tinacos',15),
  ('Casa','fosas-septicas-y-desazolve','Fosas sépticas y desazolve',16),
  ('Casa','pintores','Pintores',17),
  ('Casa','albanileria','Albañilería',18),
  ('Casa','jardineria-y-poda','Jardinería y poda',19),
  ('Casa','limpieza-domestica','Limpieza doméstica',20),
  ('Casa','limpieza-de-muebles','Limpieza de muebles',21),
  ('Casa','pisos-y-azulejo','Pisos y azulejo',22),
  ('Casa','persianas-y-cortinas','Persianas y cortinas',23),
  ('Casa','cocinas-integrales','Cocinas integrales',24),
  ('Casa','marmol-y-granito','Mármol y granito',25),
  ('Casa','albercas','Albercas',26),
  ('Casa','portones-automaticos','Portones automáticos',27),
  ('Casa','escombro-y-demolicion','Escombro y demolición',28),
  ('Casa','mudanzas-y-fletes','Mudanzas y fletes',29),
  ('Casa','aparatos-chicos','Aparatos chicos',30),
  ('Casa','palapa-y-techo-de-huano','Palapa y techo de huano',31),
  ('Auto','mecanica-a-domicilio','Mecánica a domicilio',32),
  ('Auto','taller-mecanico','Taller mecánico',33),
  ('Auto','cerrajeria-automotriz','Cerrajería automotriz',34),
  ('Auto','hojalateria-y-pintura','Hojalatería y pintura',35),
  ('Auto','cristales-y-parabrisas','Cristales y parabrisas',36),
  ('Auto','audio-y-alarmas-para-auto','Audio y alarmas para auto',37),
  ('Auto','detallado-a-domicilio','Detallado a domicilio',38),
  ('Auto','llanteras-moviles','Llanteras móviles',39),
  ('Auto','auxilio-vial-y-gruas','Auxilio vial y grúas',40),
  ('Auto','motos','Motos',41),
  ('Auto','bicicletas','Bicicletas',42),
  ('Seguridad y tecnología','camaras-cctv','Cámaras CCTV',43),
  ('Seguridad y tecnología','alarmas-y-control-de-acceso','Alarmas y control de acceso',44),
  ('Seguridad y tecnología','paneles-solares','Paneles solares',45),
  ('Seguridad y tecnología','redes-y-antenas','Redes y antenas',46),
  ('Seguridad y tecnología','computo-y-celulares','Cómputo y celulares',47),
  ('Negocios','trampas-de-grasa','Trampas de grasa',48),
  ('Otros','fumigacion','Fumigación',49),
  ('Otros','soldadura-autogena','Soldadura autógena',50),
  ('Otros','rotulacion','Rotulación',51),
  ('Otros','afilado','Afilado',52)
ON CONFLICT (slug) DO UPDATE
  SET group_name = EXCLUDED.group_name,
      name = EXCLUDED.name,
      sort_order = EXCLUDED.sort_order,
      updated_at = now();

CREATE TABLE professional_services (
  professional_id       uuid NOT NULL REFERENCES professional_profiles(user_id) ON DELETE CASCADE,
  category_id           uuid NOT NULL REFERENCES service_categories(id) ON DELETE RESTRICT,
  is_primary            boolean NOT NULL DEFAULT false,
  is_active             boolean NOT NULL DEFAULT true,
  created_at            timestamptz NOT NULL DEFAULT now(),
  PRIMARY KEY (professional_id, category_id)
);

CREATE UNIQUE INDEX professional_services_one_primary_idx
  ON professional_services (professional_id) WHERE is_primary;
CREATE INDEX professional_services_category_idx
  ON professional_services (category_id, professional_id) WHERE is_active;

-- ---------------------------------------------------------------------------
-- Marketplace: a client publishes a job, professionals quote, then the
-- client selects exactly one quote. A request stores its own address snapshot
-- so later edits to a saved address cannot rewrite job history.
-- ---------------------------------------------------------------------------
CREATE TABLE service_requests (
  id                      uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  client_id               uuid NOT NULL REFERENCES client_profiles(user_id) ON DELETE RESTRICT,
  category_id             uuid NOT NULL REFERENCES service_categories(id) ON DELETE RESTRICT,
  title                   varchar(140),
  description             text NOT NULL,
  is_urgent               boolean NOT NULL DEFAULT false,
  scheduled_for           timestamptz,
  status                  service_request_status NOT NULL DEFAULT 'open',
  assigned_professional_id uuid REFERENCES professional_profiles(user_id) ON DELETE SET NULL,
  agreed_price_mxn        numeric(10,2),
  address_line1           varchar(200) NOT NULL,
  address_line2           varchar(200),
  neighborhood            varchar(120),
  city                    varchar(120) NOT NULL,
  state                   varchar(120),
  postal_code             varchar(20),
  country_code            char(2) NOT NULL DEFAULT 'MX',
  latitude                numeric(9,6) NOT NULL CHECK (latitude BETWEEN -90 AND 90),
  longitude               numeric(9,6) NOT NULL CHECK (longitude BETWEEN -180 AND 180),
  location                geography(Point, 4326) NOT NULL,
  created_at              timestamptz NOT NULL DEFAULT now(),
  updated_at              timestamptz NOT NULL DEFAULT now(),
  quoted_at               timestamptz,
  assigned_at             timestamptz,
  accepted_at             timestamptz,
  en_route_at             timestamptz,
  started_at              timestamptz,
  completed_at            timestamptz,
  cancelled_at            timestamptz,
  cancelled_by_user_id    uuid REFERENCES app_users(id) ON DELETE SET NULL,
  cancellation_reason     text,
  expires_at              timestamptz,
  CONSTRAINT service_requests_description_not_blank CHECK (length(btrim(description)) >= 3),
  CONSTRAINT service_requests_price CHECK (agreed_price_mxn IS NULL OR agreed_price_mxn >= 0),
  CONSTRAINT service_requests_scheduled_for CHECK (scheduled_for IS NULL OR scheduled_for >= created_at - interval '1 day'),
  CONSTRAINT service_requests_assignment CHECK (
    (assigned_professional_id IS NULL AND agreed_price_mxn IS NULL)
    OR assigned_professional_id IS NOT NULL
  )
);

CREATE INDEX service_requests_client_idx ON service_requests (client_id, created_at DESC);
CREATE INDEX service_requests_professional_idx
  ON service_requests (assigned_professional_id, status, created_at DESC)
  WHERE assigned_professional_id IS NOT NULL;
CREATE INDEX service_requests_open_category_idx
  ON service_requests (category_id, is_urgent DESC, created_at DESC)
  WHERE status IN ('open', 'quoted');
CREATE INDEX service_requests_geo_idx ON service_requests USING gist (location);

CREATE TABLE service_request_photos (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  request_id            uuid NOT NULL REFERENCES service_requests(id) ON DELETE CASCADE,
  object_key            text NOT NULL,
  content_type          varchar(100),
  sort_order            smallint NOT NULL DEFAULT 0 CHECK (sort_order BETWEEN 0 AND 4),
  created_at            timestamptz NOT NULL DEFAULT now(),
  UNIQUE (request_id, object_key),
  UNIQUE (request_id, sort_order)
);

CREATE TABLE service_quotes (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  request_id            uuid NOT NULL REFERENCES service_requests(id) ON DELETE CASCADE,
  professional_id       uuid NOT NULL REFERENCES professional_profiles(user_id) ON DELETE RESTRICT,
  amount_mxn            numeric(10,2) NOT NULL CHECK (amount_mxn >= 0),
  message               text,
  estimated_arrival_minutes integer CHECK (estimated_arrival_minutes BETWEEN 0 AND 10080),
  status                quote_status NOT NULL DEFAULT 'sent',
  expires_at            timestamptz,
  created_at            timestamptz NOT NULL DEFAULT now(),
  updated_at            timestamptz NOT NULL DEFAULT now(),
  UNIQUE (request_id, professional_id)
);

-- PostgreSQL ensures no more than one professional can win a job. In the API,
-- accept a quote inside a transaction that locks the service_requests row.
CREATE UNIQUE INDEX service_quotes_one_accepted_idx
  ON service_quotes (request_id) WHERE status = 'accepted';
CREATE INDEX service_quotes_request_idx ON service_quotes (request_id, status, created_at DESC);
CREATE INDEX service_quotes_professional_idx ON service_quotes (professional_id, status, created_at DESC);

CREATE TABLE service_request_status_history (
  id                    bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  request_id            uuid NOT NULL REFERENCES service_requests(id) ON DELETE CASCADE,
  from_status           service_request_status,
  to_status             service_request_status NOT NULL,
  changed_by_user_id    uuid REFERENCES app_users(id) ON DELETE SET NULL,
  note                  text,
  created_at            timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX service_request_status_history_request_idx
  ON service_request_status_history (request_id, created_at DESC);

-- ---------------------------------------------------------------------------
-- Communication, reviews, devices, and notifications
-- ---------------------------------------------------------------------------
CREATE TABLE conversations (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  request_id            uuid UNIQUE REFERENCES service_requests(id) ON DELETE CASCADE,
  client_id             uuid NOT NULL REFERENCES client_profiles(user_id) ON DELETE CASCADE,
  professional_id       uuid NOT NULL REFERENCES professional_profiles(user_id) ON DELETE CASCADE,
  last_message_preview  varchar(500),
  last_message_at       timestamptz,
  created_at            timestamptz NOT NULL DEFAULT now(),
  updated_at            timestamptz NOT NULL DEFAULT now(),
  CONSTRAINT conversations_different_people CHECK (client_id <> professional_id)
);

CREATE UNIQUE INDEX conversations_pair_without_request_idx
  ON conversations (client_id, professional_id) WHERE request_id IS NULL;
CREATE INDEX conversations_client_idx ON conversations (client_id, last_message_at DESC NULLS LAST);
CREATE INDEX conversations_professional_idx ON conversations (professional_id, last_message_at DESC NULLS LAST);

CREATE TABLE messages (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  conversation_id       uuid NOT NULL REFERENCES conversations(id) ON DELETE CASCADE,
  sender_id             uuid NOT NULL REFERENCES app_users(id) ON DELETE RESTRICT,
  body                  text NOT NULL,
  read_at               timestamptz,
  deleted_at            timestamptz,
  created_at            timestamptz NOT NULL DEFAULT now(),
  CONSTRAINT messages_body_not_blank CHECK (length(btrim(body)) BETWEEN 1 AND 4000)
);

CREATE INDEX messages_conversation_idx ON messages (conversation_id, created_at DESC);

CREATE TABLE professional_evidence (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  professional_id       uuid NOT NULL REFERENCES professional_profiles(user_id) ON DELETE CASCADE,
  object_key            text NOT NULL,
  description           varchar(500),
  is_approved           boolean NOT NULL DEFAULT false,
  is_public             boolean NOT NULL DEFAULT false,
  reviewed_by_user_id   uuid REFERENCES app_users(id) ON DELETE SET NULL,
  reviewed_at           timestamptz,
  created_at            timestamptz NOT NULL DEFAULT now(),
  UNIQUE (professional_id, object_key),
  CONSTRAINT professional_evidence_visibility CHECK (NOT is_public OR is_approved)
);

CREATE INDEX professional_evidence_public_idx
  ON professional_evidence (professional_id, created_at DESC)
  WHERE is_approved AND is_public;

-- Both parties may review after completion. The ASP.NET service must verify
-- reviewer/recipient match the request participants before inserting a row.
CREATE TABLE reviews (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  request_id            uuid NOT NULL REFERENCES service_requests(id) ON DELETE CASCADE,
  reviewer_id           uuid NOT NULL REFERENCES app_users(id) ON DELETE RESTRICT,
  recipient_id          uuid NOT NULL REFERENCES app_users(id) ON DELETE RESTRICT,
  recipient_role        user_role NOT NULL CHECK (recipient_role IN ('client', 'professional')),
  rating                smallint NOT NULL CHECK (rating BETWEEN 1 AND 5),
  comment               varchar(1000),
  created_at            timestamptz NOT NULL DEFAULT now(),
  UNIQUE (request_id, reviewer_id),
  CONSTRAINT reviews_not_self CHECK (reviewer_id <> recipient_id)
);

CREATE INDEX reviews_professional_idx
  ON reviews (recipient_id, created_at DESC) WHERE recipient_role = 'professional';

CREATE TABLE push_devices (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id               uuid NOT NULL REFERENCES app_users(id) ON DELETE CASCADE,
  push_token            text NOT NULL UNIQUE,
  platform              device_platform NOT NULL,
  device_name           varchar(160),
  is_active             boolean NOT NULL DEFAULT true,
  last_seen_at          timestamptz NOT NULL DEFAULT now(),
  created_at            timestamptz NOT NULL DEFAULT now(),
  updated_at            timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX push_devices_active_user_idx ON push_devices (user_id) WHERE is_active;

CREATE TABLE notifications (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id               uuid NOT NULL REFERENCES app_users(id) ON DELETE CASCADE,
  request_id            uuid REFERENCES service_requests(id) ON DELETE CASCADE,
  channel               notification_channel NOT NULL DEFAULT 'in_app',
  type                  varchar(80) NOT NULL,
  title                 varchar(160) NOT NULL,
  body                  varchar(600) NOT NULL,
  data                  jsonb NOT NULL DEFAULT '{}'::jsonb,
  sent_at               timestamptz,
  read_at               timestamptz,
  failed_at             timestamptz,
  failure_reason        text,
  created_at            timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX notifications_unread_idx
  ON notifications (user_id, created_at DESC) WHERE read_at IS NULL;
CREATE INDEX notifications_request_idx ON notifications (request_id, created_at DESC);

-- ---------------------------------------------------------------------------
-- Stripe subscriptions and immutable webhook receipt log
-- ---------------------------------------------------------------------------
CREATE TABLE provider_subscriptions (
  id                    uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  professional_id       uuid NOT NULL REFERENCES professional_profiles(user_id) ON DELETE CASCADE,
  stripe_customer_id    text NOT NULL,
  stripe_subscription_id text NOT NULL UNIQUE,
  stripe_price_id       text NOT NULL,
  amount_mxn            integer NOT NULL CHECK (amount_mxn IN (200, 300)),
  status                subscription_status NOT NULL,
  current_period_start  timestamptz,
  current_period_end    timestamptz,
  trial_ends_at         timestamptz,
  cancel_at_period_end  boolean NOT NULL DEFAULT false,
  cancelled_at          timestamptz,
  created_at            timestamptz NOT NULL DEFAULT now(),
  updated_at            timestamptz NOT NULL DEFAULT now(),
  CONSTRAINT provider_subscriptions_period CHECK (
    current_period_end IS NULL OR current_period_start IS NULL OR current_period_end > current_period_start
  )
);

CREATE UNIQUE INDEX provider_subscriptions_one_live_idx
  ON provider_subscriptions (professional_id)
  WHERE status IN ('trialing', 'active', 'past_due', 'paused');
CREATE INDEX provider_subscriptions_customer_idx ON provider_subscriptions (stripe_customer_id);

-- Insert the Stripe event receipt in the same database transaction as the
-- subscription update. The primary key makes Stripe webhook retries idempotent.
CREATE TABLE stripe_webhook_events (
  stripe_event_id       text PRIMARY KEY,
  event_type            varchar(160) NOT NULL,
  api_version           varchar(80),
  payload               jsonb NOT NULL,
  received_at           timestamptz NOT NULL DEFAULT now(),
  processed_at          timestamptz,
  processing_error      text
);

CREATE TABLE audit_events (
  id                    bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  actor_user_id         uuid REFERENCES app_users(id) ON DELETE SET NULL,
  entity_type           varchar(80) NOT NULL,
  entity_id             uuid,
  action                varchar(120) NOT NULL,
  metadata              jsonb NOT NULL DEFAULT '{}'::jsonb,
  ip_address            inet,
  created_at            timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX audit_events_entity_idx ON audit_events (entity_type, entity_id, created_at DESC);
CREATE INDEX audit_events_actor_idx ON audit_events (actor_user_id, created_at DESC);

-- ---------------------------------------------------------------------------
-- Database-owned integrity helpers. They do not authorize a caller; ASP.NET
-- remains the authorization boundary. These triggers prevent accidental data
-- corruption if an internal job or EF Core update bypasses service code.
-- ---------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION set_updated_at()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  NEW.updated_at := now();
  RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION set_location_point()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  NEW.location := ST_SetSRID(ST_MakePoint(NEW.longitude, NEW.latitude), 4326)::geography;
  RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION stamp_service_request_transition()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.status IS DISTINCT FROM OLD.status THEN
    INSERT INTO service_request_status_history (request_id, from_status, to_status)
      VALUES (NEW.id, OLD.status, NEW.status);

    CASE NEW.status
      WHEN 'quoted' THEN NEW.quoted_at := COALESCE(NEW.quoted_at, now());
      WHEN 'assigned' THEN NEW.assigned_at := COALESCE(NEW.assigned_at, now());
      WHEN 'accepted' THEN NEW.accepted_at := COALESCE(NEW.accepted_at, now());
      WHEN 'en_route' THEN NEW.en_route_at := COALESCE(NEW.en_route_at, now());
      WHEN 'in_progress' THEN NEW.started_at := COALESCE(NEW.started_at, now());
      WHEN 'completed' THEN NEW.completed_at := COALESCE(NEW.completed_at, now());
      WHEN 'cancelled' THEN NEW.cancelled_at := COALESCE(NEW.cancelled_at, now());
      ELSE NULL;
    END CASE;
  END IF;
  RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION refresh_professional_rating()
RETURNS trigger LANGUAGE plpgsql AS $$
DECLARE
  target_id uuid;
BEGIN
  IF TG_OP = 'DELETE' THEN
    target_id := OLD.recipient_id;
  ELSE
    target_id := NEW.recipient_id;
  END IF;
  UPDATE professional_profiles p
  SET rating_average = COALESCE((
        SELECT round(avg(r.rating)::numeric, 2)
        FROM reviews r
        WHERE r.recipient_id = target_id AND r.recipient_role = 'professional'
      ), 0),
      rating_count = (
        SELECT count(*)
        FROM reviews r
        WHERE r.recipient_id = target_id AND r.recipient_role = 'professional'
      ),
      updated_at = now()
  WHERE p.user_id = target_id;
  IF TG_OP = 'DELETE' THEN
    RETURN OLD;
  END IF;
  RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION validate_quote_professional_service()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NOT EXISTS (
    SELECT 1
    FROM service_requests r
    JOIN professional_services ps
      ON ps.professional_id = NEW.professional_id
     AND ps.category_id = r.category_id
     AND ps.is_active
    JOIN professional_profiles p ON p.user_id = NEW.professional_id
    WHERE r.id = NEW.request_id
      AND r.status IN ('open', 'quoted')
      AND p.verification = 'verified'
      AND p.subscription_active
  ) THEN
    RAISE EXCEPTION 'Professional cannot quote this request';
  END IF;
  RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION update_conversation_summary()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  UPDATE conversations
  SET last_message_preview = left(NEW.body, 500),
      last_message_at = NEW.created_at,
      updated_at = now()
  WHERE id = NEW.conversation_id;
  RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION increment_completed_jobs()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
  IF NEW.status = 'completed' AND OLD.status IS DISTINCT FROM 'completed'
     AND NEW.assigned_professional_id IS NOT NULL THEN
    UPDATE professional_profiles
    SET completed_jobs_count = completed_jobs_count + 1,
        updated_at = now()
    WHERE user_id = NEW.assigned_professional_id;
  END IF;
  RETURN NEW;
END;
$$;

CREATE TRIGGER app_users_updated_at
  BEFORE UPDATE ON app_users FOR EACH ROW EXECUTE FUNCTION set_updated_at();
CREATE TRIGGER client_profiles_updated_at
  BEFORE UPDATE ON client_profiles FOR EACH ROW EXECUTE FUNCTION set_updated_at();
CREATE TRIGGER professional_profiles_updated_at
  BEFORE UPDATE ON professional_profiles FOR EACH ROW EXECUTE FUNCTION set_updated_at();
CREATE TRIGGER client_addresses_updated_at
  BEFORE UPDATE ON client_addresses FOR EACH ROW EXECUTE FUNCTION set_updated_at();
CREATE TRIGGER service_categories_updated_at
  BEFORE UPDATE ON service_categories FOR EACH ROW EXECUTE FUNCTION set_updated_at();
CREATE TRIGGER service_requests_updated_at
  BEFORE UPDATE ON service_requests FOR EACH ROW EXECUTE FUNCTION set_updated_at();
CREATE TRIGGER service_quotes_updated_at
  BEFORE UPDATE ON service_quotes FOR EACH ROW EXECUTE FUNCTION set_updated_at();
CREATE TRIGGER conversations_updated_at
  BEFORE UPDATE ON conversations FOR EACH ROW EXECUTE FUNCTION set_updated_at();
CREATE TRIGGER push_devices_updated_at
  BEFORE UPDATE ON push_devices FOR EACH ROW EXECUTE FUNCTION set_updated_at();
CREATE TRIGGER provider_subscriptions_updated_at
  BEFORE UPDATE ON provider_subscriptions FOR EACH ROW EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER professional_locations_set_point
  BEFORE INSERT OR UPDATE OF latitude, longitude ON professional_locations
  FOR EACH ROW EXECUTE FUNCTION set_location_point();
CREATE TRIGGER client_addresses_set_point
  BEFORE INSERT OR UPDATE OF latitude, longitude ON client_addresses
  FOR EACH ROW EXECUTE FUNCTION set_location_point();
CREATE TRIGGER service_requests_set_point
  BEFORE INSERT OR UPDATE OF latitude, longitude ON service_requests
  FOR EACH ROW EXECUTE FUNCTION set_location_point();
CREATE TRIGGER service_requests_stamp_transition
  BEFORE UPDATE OF status ON service_requests
  FOR EACH ROW EXECUTE FUNCTION stamp_service_request_transition();
CREATE TRIGGER service_quotes_validate_professional
  BEFORE INSERT OR UPDATE OF request_id, professional_id ON service_quotes
  FOR EACH ROW EXECUTE FUNCTION validate_quote_professional_service();
CREATE TRIGGER reviews_refresh_professional_rating
  AFTER INSERT OR UPDATE OR DELETE ON reviews
  FOR EACH ROW EXECUTE FUNCTION refresh_professional_rating();
CREATE TRIGGER messages_update_conversation
  AFTER INSERT ON messages
  FOR EACH ROW EXECUTE FUNCTION update_conversation_summary();
CREATE TRIGGER service_requests_increment_completed_jobs
  AFTER UPDATE OF status ON service_requests
  FOR EACH ROW EXECUTE FUNCTION increment_completed_jobs();

-- ---------------------------------------------------------------------------
-- Operational notes for the ASP.NET implementation
-- ---------------------------------------------------------------------------
-- 1. Quote acceptance must be one transaction:
--      SELECT ... FROM service_requests WHERE id = @id FOR UPDATE;
--      UPDATE service_quotes SET status = 'accepted' WHERE id = @quoteId;
--      UPDATE service_quotes SET status = 'declined' WHERE request_id = @id
--        AND id <> @quoteId AND status = 'sent';
--      UPDATE service_requests SET assigned_professional_id = @professionalId,
--        agreed_price_mxn = @amount, status = 'assigned' WHERE id = @id;
-- 2. The API must confirm request ownership/participation before returning the
--    exact service address, photos, messages, notifications, or signed media
--    URL. Do not place permanent URLs in this schema or API responses.
-- 3. On verified Stripe webhook handling, insert stripe_event_id first inside
--    the transaction. If it already exists, return HTTP 200 without repeating
--    work. Sync professional_profiles.subscription_active from the current
--    provider_subscriptions record in that same transaction.
-- 4. Register one transaction-safe application database account, revoke
--    CONNECT/USAGE from public if your hosting provider permits it, and keep
--    the connection string only in server-side configuration/secrets.
-- 5. Use EF Core migrations for all later alterations. Do not modify this
--    bootstrap file after it has been applied in any deployed environment.

COMMIT;
