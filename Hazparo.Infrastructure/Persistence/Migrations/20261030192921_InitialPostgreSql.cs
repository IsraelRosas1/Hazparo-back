using System;
using Hazparo.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hazparo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:account_status", "active,deleted,pending,suspended")
                .Annotation("Npgsql:Enum:quote_status", "accepted,declined,expired,sent,withdrawn")
                .Annotation("Npgsql:Enum:service_request_status", "accepted,assigned,cancelled,completed,draft,en_route,expired,in_progress,open,quoted")
                .Annotation("Npgsql:Enum:user_role", "admin,client,professional")
                .Annotation("Npgsql:Enum:verification_status", "not_submitted,pending,rejected,verified")
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "app_users",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    email = table.Column<string>(type: "citext", nullable: false),
                    normalized_email = table.Column<string>(type: "text", nullable: true, computedColumnSql: "upper(email::text)", stored: true),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    avatar_object_key = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<AccountStatus>(type: "account_status", nullable: false, defaultValue: AccountStatus.Pending),
                    email_confirmed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    phone_confirmed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_login_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "service_categories",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    group_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(140)", maxLength: 140, nullable: false),
                    icon_key = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    search_aliases = table.Column<string[]>(type: "text[]", nullable: false, defaultValueSql: "'{}'::text[]"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    sort_order = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_user_roles",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<UserRole>(type: "user_role", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_user_roles", x => new { x.user_id, x.role });
                    table.ForeignKey(
                        name: "fk_app_user_roles_app_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "client_profiles",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    default_city = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_client_profiles", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_client_profiles_app_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "professional_profiles",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    business_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    years_experience = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    hourly_rate_mxn = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    is_available = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    accepts_emergency = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    service_radius_km = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)10),
                    verification = table.Column<VerificationStatus>(type: "verification_status", nullable: false, defaultValue: VerificationStatus.NotSubmitted),
                    verified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    verification_notes = table.Column<string>(type: "text", nullable: true),
                    stripe_customer_id = table.Column<string>(type: "text", nullable: true),
                    subscription_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    subscription_expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rating_average = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false, defaultValue: 0m),
                    rating_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    completed_jobs_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_professional_profiles", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_professional_profiles_app_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "client_addresses",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false, defaultValue: "Casa"),
                    recipient_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    address_line1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address_line2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    neighborhood = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    city = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    state = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    country_code = table.Column<string>(type: "char(2)", nullable: false, defaultValue: "MX"),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    location = table.Column<Point>(type: "geography (point,4326)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_client_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_client_addresses_client_profiles_client_id",
                        column: x => x.client_id,
                        principalSchema: "public",
                        principalTable: "client_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "professional_locations",
                schema: "public",
                columns: table => new
                {
                    professional_id = table.Column<Guid>(type: "uuid", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    location = table.Column<Point>(type: "geography (point,4326)", nullable: false),
                    city = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_professional_locations", x => x.professional_id);
                    table.ForeignKey(
                        name: "fk_professional_locations_professional_profiles_professional_id",
                        column: x => x.professional_id,
                        principalSchema: "public",
                        principalTable: "professional_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "professional_services",
                schema: "public",
                columns: table => new
                {
                    professional_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_professional_services", x => new { x.professional_id, x.category_id });
                    table.ForeignKey(
                        name: "fk_professional_services_professional_profiles_professional_id",
                        column: x => x.professional_id,
                        principalSchema: "public",
                        principalTable: "professional_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_professional_services_service_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "public",
                        principalTable: "service_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "service_requests",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(140)", maxLength: 140, nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_urgent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    scheduled_for = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<ServiceRequestStatus>(type: "service_request_status", nullable: false, defaultValue: ServiceRequestStatus.Open),
                    assigned_professional_id = table.Column<Guid>(type: "uuid", nullable: true),
                    agreed_price_mxn = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    address_line1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address_line2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    neighborhood = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    city = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    state = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    country_code = table.Column<string>(type: "char(2)", nullable: false, defaultValue: "MX"),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    location = table.Column<Point>(type: "geography (point,4326)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    quoted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    assigned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    accepted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    en_route_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cancellation_reason = table.Column<string>(type: "text", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_service_requests_app_users_cancelled_by_user_id",
                        column: x => x.cancelled_by_user_id,
                        principalSchema: "public",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_service_requests_client_profiles_client_id",
                        column: x => x.client_id,
                        principalSchema: "public",
                        principalTable: "client_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_service_requests_professional_profiles_assigned_professiona",
                        column: x => x.assigned_professional_id,
                        principalSchema: "public",
                        principalTable: "professional_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_service_requests_service_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "public",
                        principalTable: "service_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "service_quotes",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    professional_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_mxn = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    estimated_arrival_minutes = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<QuoteStatus>(type: "quote_status", nullable: false, defaultValue: QuoteStatus.Sent),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_quotes", x => x.id);
                    table.ForeignKey(
                        name: "fk_service_quotes_professional_profiles_professional_id",
                        column: x => x.professional_id,
                        principalSchema: "public",
                        principalTable: "professional_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_service_quotes_service_requests_request_id",
                        column: x => x.request_id,
                        principalSchema: "public",
                        principalTable: "service_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_request_photos",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    object_key = table.Column<string>(type: "text", nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    sort_order = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_request_photos", x => x.id);
                    table.ForeignKey(
                        name: "fk_service_request_photos_service_requests_request_id",
                        column: x => x.request_id,
                        principalSchema: "public",
                        principalTable: "service_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_request_status_history",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_status = table.Column<ServiceRequestStatus>(type: "service_request_status", nullable: true),
                    to_status = table.Column<ServiceRequestStatus>(type: "service_request_status", nullable: false),
                    changed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_request_status_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_service_request_status_history_app_users_changed_by_user_id",
                        column: x => x.changed_by_user_id,
                        principalSchema: "public",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_service_request_status_history_service_requests_request_id",
                        column: x => x.request_id,
                        principalSchema: "public",
                        principalTable: "service_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_client_addresses_client_id",
                schema: "public",
                table: "client_addresses",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_professional_services_category_id",
                schema: "public",
                table: "professional_services",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_quotes_professional_id",
                schema: "public",
                table: "service_quotes",
                column: "professional_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_quotes_request_id",
                schema: "public",
                table: "service_quotes",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_request_photos_request_id",
                schema: "public",
                table: "service_request_photos",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_request_status_history_changed_by_user_id",
                schema: "public",
                table: "service_request_status_history",
                column: "changed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_request_status_history_request_id",
                schema: "public",
                table: "service_request_status_history",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_requests_assigned_professional_id",
                schema: "public",
                table: "service_requests",
                column: "assigned_professional_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_requests_cancelled_by_user_id",
                schema: "public",
                table: "service_requests",
                column: "cancelled_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_requests_category_id",
                schema: "public",
                table: "service_requests",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_requests_client_id",
                schema: "public",
                table: "service_requests",
                column: "client_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_user_roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "client_addresses",
                schema: "public");

            migrationBuilder.DropTable(
                name: "professional_locations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "professional_services",
                schema: "public");

            migrationBuilder.DropTable(
                name: "service_quotes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "service_request_photos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "service_request_status_history",
                schema: "public");

            migrationBuilder.DropTable(
                name: "service_requests",
                schema: "public");

            migrationBuilder.DropTable(
                name: "client_profiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "professional_profiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "service_categories",
                schema: "public");

            migrationBuilder.DropTable(
                name: "app_users",
                schema: "public");
        }
    }
}
