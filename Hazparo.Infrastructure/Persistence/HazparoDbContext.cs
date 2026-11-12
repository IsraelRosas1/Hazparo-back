using System.Linq.Expressions;
using Hazparo.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hazparo.Infrastructure.Persistence;

public class HazparoDbContext(DbContextOptions<HazparoDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<ClientProfile> ClientProfiles => Set<ClientProfile>();
    public DbSet<ClientAddress> ClientAddresses => Set<ClientAddress>();
    public DbSet<ProfessionalProfile> ProfessionalProfiles => Set<ProfessionalProfile>();
    public DbSet<ProfessionalLocation> ProfessionalLocations => Set<ProfessionalLocation>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<ProfessionalService> ProfessionalServices => Set<ProfessionalService>();
    public DbSet<ProfessionalVerificationDocument> ProfessionalVerificationDocuments => Set<ProfessionalVerificationDocument>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<ServiceRequestPhoto> ServiceRequestPhotos => Set<ServiceRequestPhoto>();
    public DbSet<ServiceQuote> ServiceQuotes => Set<ServiceQuote>();
    public DbSet<ServiceRequestStatusHistory> ServiceRequestStatusHistories =>
        Set<ServiceRequestStatusHistory>();
    public DbSet<AuthRefreshToken> AuthRefreshTokens =>
        Set<AuthRefreshToken>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>().ToTable("app_users");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("app_roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("app_user_roles");

        modelBuilder.HasDefaultSchema("public");
        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.HasPostgresExtension("citext");

        ConfigureClientProfile(modelBuilder);
        ConfigureClientAddress(modelBuilder);
        ConfigureProfessionalProfile(modelBuilder);
        ConfigureProfessionalLocation(modelBuilder);
        ConfigureServiceCategory(modelBuilder);
        ConfigureProfessionalService(modelBuilder);
        ConfigureServiceRequest(modelBuilder);
        ConfigureServiceRequestPhoto(modelBuilder);
        ConfigureServiceQuote(modelBuilder);
        ConfigureServiceRequestStatusHistory(modelBuilder);
        ConfigureAuthRefreshToken(modelBuilder);
        ConfigureProfessionalVerificationDocument(modelBuilder);
    }

    private static void ConfigureClientProfile(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ClientProfile>();

        entity.ToTable("client_profiles");
        entity.HasKey(profile => profile.UserId);
        entity.Property(profile => profile.UserId).HasColumnName("user_id").HasColumnType("uuid");
        entity.Property(profile => profile.DefaultCity)
            .HasColumnName("default_city")
            .HasMaxLength(120);
        ConfigureCreatedAndUpdated(entity, profile => profile.CreatedAt, profile => profile.UpdatedAt);

        entity.HasOne(profile => profile.User)
            .WithOne(user => user.ClientProfile)
            .HasForeignKey<ClientProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    private static void ConfigureAuthRefreshToken(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<AuthRefreshToken>();

        entity.ToTable("auth_refresh_tokens", table =>
        {
            table.HasCheckConstraint(
                "ck_auth_refresh_tokens_expiry",
                "expires_at > created_at");
        });

        entity.HasKey(token => token.Id);

        entity.Property(token => token.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");

        entity.Property(token => token.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uuid");

        entity.Property(token => token.TokenHash)
            .HasColumnName("token_hash")
            .HasColumnType("char(64)")
            .IsRequired();

        entity.Property(token => token.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        entity.Property(token => token.RevokedAt)
            .HasColumnName("revoked_at");

        entity.Property(token => token.ReplacedByTokenId)
            .HasColumnName("replaced_by_token_id")
            .HasColumnType("uuid");

        entity.Property(token => token.UserAgent)
            .HasColumnName("user_agent")
            .HasColumnType("text");

        entity.Property(token => token.IpAddress)
            .HasColumnName("ip_address")
            .HasColumnType("inet");

        entity.Property(token => token.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        entity.HasIndex(token => token.TokenHash)
            .IsUnique();

        entity.HasIndex(token => new { token.UserId, token.ExpiresAt })
            .HasDatabaseName("auth_refresh_tokens_lookup_idx")
            .HasFilter("revoked_at IS NULL");

        entity.HasOne(token => token.User)
            .WithMany(user => user.RefreshTokens)
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(token => token.ReplacedByToken)
            .WithMany()
            .HasForeignKey(token => token.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void ConfigureClientAddress(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ClientAddress>();

        entity.ToTable("client_addresses");
        entity.HasKey(address => address.Id);
        entity.Property(address => address.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");
        entity.Property(address => address.ClientId).HasColumnName("client_id").HasColumnType("uuid");
        entity.Property(address => address.Label)
            .HasMaxLength(80)
            .HasDefaultValue("Casa")
            .IsRequired();
        entity.Property(address => address.RecipientName).HasColumnName("recipient_name").HasMaxLength(120);
        entity.Property(address => address.PhoneNumber).HasColumnName("phone_number").HasMaxLength(30);
        entity.Property(address => address.AddressLine1)
            .HasColumnName("address_line1")
            .HasMaxLength(200)
            .IsRequired();
        entity.Property(address => address.AddressLine2).HasColumnName("address_line2").HasMaxLength(200);
        entity.Property(address => address.Neighborhood).HasMaxLength(120);
        entity.Property(address => address.City).HasMaxLength(120).IsRequired();
        entity.Property(address => address.State).HasMaxLength(120);
        entity.Property(address => address.PostalCode).HasColumnName("postal_code").HasMaxLength(20);
        entity.Property(address => address.CountryCode)
            .HasColumnName("country_code")
            .HasColumnType("char(2)")
            .HasDefaultValue("MX")
            .IsRequired();
        entity.Property(address => address.Latitude).HasPrecision(9,6).IsRequired();
        entity.Property(address=>address.Longitude).HasPrecision(9,6).IsRequired();
        entity.Property(address => address.Location)
            .HasColumnName("location")
            .HasColumnType("geography (point,4326)")
            .IsRequired();
        entity.Property(address => address.IsDefault)
            .HasColumnName("is_default")
            .HasDefaultValue(false)
            .IsRequired();
        ConfigureCreatedAndUpdated(entity, address => address.CreatedAt, address => address.UpdatedAt);

        entity.HasOne(address => address.Client)
            .WithMany(client => client.Addresses)
            .HasForeignKey(address => address.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureProfessionalProfile(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ProfessionalProfile>();

        entity.ToTable("professional_profiles");
        entity.HasKey(profile => profile.UserId);
        entity.Property(profile => profile.UserId).HasColumnName("user_id").HasColumnType("uuid");
        entity.Property(profile => profile.BusinessName).HasColumnName("business_name").HasMaxLength(160);
        entity.Property(profile => profile.Bio).HasColumnType("text");
        entity.Property(profile => profile.YearsExperience)
            .HasColumnName("years_experience")
            .HasDefaultValue((short)0)
            .IsRequired();
        entity.Property(profile => profile.HourlyRateMxn)
            .HasColumnName("hourly_rate_mxn")
            .HasPrecision(10, 2);
        entity.Property(profile => profile.IsAvailable)
            .HasColumnName("is_available")
            .HasDefaultValue(false)
            .IsRequired();
        entity.Property(profile => profile.AcceptsEmergency)
            .HasColumnName("accepts_emergency")
            .HasDefaultValue(false)
            .IsRequired();
        entity.Property(profile => profile.ServiceRadiusKm)
            .HasColumnName("service_radius_km")
            .HasDefaultValue((short)10)
            .IsRequired();
        entity.Property(profile => profile.Verification)
            .HasColumnName("verification")
            .HasDefaultValue(VerificationStatus.NotSubmitted)
            .IsRequired();
        entity.Property(profile => profile.VerifiedAt).HasColumnName("verified_at");
        entity.Property(profile => profile.VerificationNotes)
            .HasColumnName("verification_notes")
            .HasColumnType("text");
        entity.Property(profile => profile.StripeCustomerId)
            .HasColumnName("stripe_customer_id")
            .HasColumnType("text");
        entity.Property(profile => profile.SubscriptionActive)
            .HasColumnName("subscription_active")
            .HasDefaultValue(false)
            .IsRequired();
        entity.Property(profile => profile.SubscriptionExpiresAt).HasColumnName("subscription_expires_at");
        entity.Property(profile => profile.RatingAverage)
            .HasColumnName("rating_average")
            .HasPrecision(3, 2)
            .HasDefaultValue(0m)
            .IsRequired();
        entity.Property(profile => profile.RatingCount)
            .HasColumnName("rating_count")
            .HasDefaultValue(0)
            .IsRequired();
        entity.Property(profile => profile.CompletedJobsCount)
            .HasColumnName("completed_jobs_count")
            .HasDefaultValue(0)
            .IsRequired();
        ConfigureCreatedAndUpdated(entity, profile => profile.CreatedAt, profile => profile.UpdatedAt);

        entity.HasOne(profile => profile.User)
            .WithOne(user => user.ProfessionalProfile)
            .HasForeignKey<ProfessionalProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureProfessionalLocation(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ProfessionalLocation>();

        entity.ToTable("professional_locations");
        entity.HasKey(location => location.ProfessionalId);
        entity.Property(location => location.ProfessionalId)
            .HasColumnName("professional_id")
            .HasColumnType("uuid");
        entity.Property(address => address.Latitude).HasPrecision(9,6).IsRequired();
        entity.Property(address=>address.Longitude).HasPrecision(9,6).IsRequired();
        entity.Property(location => location.Location)
            .HasColumnName("location")
            .HasColumnType("geography (point,4326)")
            .IsRequired();
        entity.Property(location => location.City).HasMaxLength(120);
        entity.Property(location => location.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        entity.HasOne(location => location.Professional)
            .WithOne(profile => profile.Location)
            .HasForeignKey<ProfessionalLocation>(location => location.ProfessionalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    private static void ConfigureProfessionalVerificationDocument(
    ModelBuilder modelBuilder)
    {
        var entity =
            modelBuilder.Entity<ProfessionalVerificationDocument>();

        entity.ToTable("professional_verification_documents");

        entity.HasKey(document => document.Id);

        entity.Property(document => document.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");

        entity.Property(document => document.ProfessionalId)
            .HasColumnName("professional_id")
            .HasColumnType("uuid")
            .IsRequired();

        entity.Property(document => document.DocumentType)
            .HasColumnName("document_type")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        entity.Property(document => document.StorageKey)
            .HasColumnName("storage_key")
            .HasMaxLength(500)
            .IsRequired();

        entity.Property(document => document.OriginalFileName)
            .HasColumnName("original_file_name")
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(document => document.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(document => document.SizeBytes)
            .HasColumnName("size_bytes")
            .IsRequired();

        entity.Property(document => document.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        entity.HasIndex(document =>
            new
            {
                document.ProfessionalId,
                document.DocumentType
            })
            .IsUnique();

        entity.HasOne(document => document.Professional)
            .WithMany(profile => profile.VerificationDocuments)
            .HasForeignKey(document => document.ProfessionalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    private static void ConfigureServiceCategory(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ServiceCategory>();

        entity.ToTable("service_categories");
        entity.HasKey(category => category.Id);
        entity.Property(category => category.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");
        entity.Property(category => category.GroupName)
            .HasColumnName("group_name")
            .HasMaxLength(100)
            .IsRequired();
        entity.Property(category => category.Slug).HasMaxLength(100).IsRequired();
        entity.Property(category => category.Name).HasMaxLength(140).IsRequired();
        entity.Property(category => category.IconKey).HasColumnName("icon_key").HasMaxLength(80);
        entity.Property(category => category.SearchAliases)
            .HasColumnName("search_aliases")
            .HasColumnType("text[]")
            .HasDefaultValueSql("'{}'::text[]")
            .IsRequired();
        entity.Property(category => category.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();
        entity.Property(category => category.SortOrder).HasColumnName("sort_order").IsRequired();
        ConfigureCreatedAndUpdated(entity, category => category.CreatedAt, category => category.UpdatedAt);
        entity.HasIndex(category => category.Slug).IsUnique();
        
    }

    private static void ConfigureProfessionalService(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ProfessionalService>();

        entity.ToTable("professional_services");
        entity.HasKey(service => new { service.ProfessionalId, service.CategoryId });
        entity.Property(service => service.ProfessionalId)
            .HasColumnName("professional_id")
            .HasColumnType("uuid");
        entity.Property(service => service.CategoryId)
            .HasColumnName("category_id")
            .HasColumnType("uuid");
        entity.Property(service => service.IsPrimary)
            .HasColumnName("is_primary")
            .HasDefaultValue(false)
            .IsRequired();
        entity.Property(service => service.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();
        entity.Property(service => service.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        entity.HasOne(service => service.Professional)
            .WithMany(profile => profile.Services)
            .HasForeignKey(service => service.ProfessionalId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(service => service.Category)
            .WithMany(category => category.ProfessionalServices)
            .HasForeignKey(service => service.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureServiceRequest(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ServiceRequest>();

        entity.ToTable("service_requests");
        entity.HasKey(request => request.Id);
        entity.Property(request => request.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");
        entity.Property(request => request.ClientId)
            .HasColumnName("client_id")
            .HasColumnType("uuid");
        entity.Property(request => request.CategoryId)
            .HasColumnName("category_id")
            .HasColumnType("uuid");
        entity.Property(request => request.Title).HasMaxLength(140);
        entity.Property(request => request.Description).HasColumnType("text").IsRequired();
        entity.Property(request => request.IsUrgent)
            .HasColumnName("is_urgent")
            .HasDefaultValue(false)
            .IsRequired();
        entity.Property(request => request.ScheduledFor).HasColumnName("scheduled_for");
        entity.Property(request => request.Status)
            .HasDefaultValue(ServiceRequestStatus.Open)
            .HasSentinel(ServiceRequestStatus.Open)
            .IsRequired();
        entity.Property(request => request.AssignedProfessionalId)
            .HasColumnName("assigned_professional_id")
            .HasColumnType("uuid");
        entity.Property(request => request.AgreedPriceMxn)
            .HasColumnName("agreed_price_mxn")
            .HasPrecision(10, 2);
        entity.Property(request => request.AddressLine1)
            .HasColumnName("address_line1")
            .HasMaxLength(200)
            .IsRequired();
        entity.Property(request => request.AddressLine2)
            .HasColumnName("address_line2")
            .HasMaxLength(200);
        entity.Property(request => request.Neighborhood).HasMaxLength(120);
        entity.Property(request => request.City).HasMaxLength(120).IsRequired();
        entity.Property(request => request.State).HasMaxLength(120);
        entity.Property(request => request.PostalCode).HasColumnName("postal_code").HasMaxLength(20);
        entity.Property(request => request.CountryCode)
            .HasColumnName("country_code")
            .HasColumnType("char(2)")
            .HasDefaultValue("MX")
            .IsRequired();
        entity.Property(address => address.Latitude).HasPrecision(9,6).IsRequired();
        entity.Property(address=>address.Longitude).HasPrecision(9,6).IsRequired();
        entity.Property(request => request.Location)
            .HasColumnName("location")
            .HasColumnType("geography (point,4326)")
            .IsRequired();
        ConfigureCreatedAndUpdated(entity, request => request.CreatedAt, request => request.UpdatedAt);
        entity.Property(request => request.QuotedAt).HasColumnName("quoted_at");
        entity.Property(request => request.AssignedAt).HasColumnName("assigned_at");
        entity.Property(request => request.AcceptedAt).HasColumnName("accepted_at");
        entity.Property(request => request.EnRouteAt).HasColumnName("en_route_at");
        entity.Property(request => request.StartedAt).HasColumnName("started_at");
        entity.Property(request => request.CompletedAt).HasColumnName("completed_at");
        entity.Property(request => request.CancelledAt).HasColumnName("cancelled_at");
        entity.Property(request => request.CancelledByUserId)
            .HasColumnName("cancelled_by_user_id")
            .HasColumnType("uuid");
        entity.Property(request => request.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasColumnType("text");
        entity.Property(request => request.ExpiresAt).HasColumnName("expires_at");

        entity.HasOne(request => request.Client)
            .WithMany(client => client.ServiceRequests)
            .HasForeignKey(request => request.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(request => request.Category)
            .WithMany(category => category.ServiceRequests)
            .HasForeignKey(request => request.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(request => request.AssignedProfessional)
            .WithMany(profile => profile.AssignedRequests)
            .HasForeignKey(request => request.AssignedProfessionalId)
            .OnDelete(DeleteBehavior.SetNull);
        entity.HasOne(request => request.CancelledByUser)
            .WithMany()
            .HasForeignKey(request => request.CancelledByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void ConfigureServiceRequestPhoto(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ServiceRequestPhoto>();

        entity.ToTable("service_request_photos");
        entity.HasKey(photo => photo.Id);
        entity.Property(photo => photo.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");
        entity.Property(photo => photo.RequestId)
            .HasColumnName("request_id")
            .HasColumnType("uuid");
        entity.Property(photo => photo.ObjectKey)
            .HasColumnName("object_key")
            .HasColumnType("text")
            .IsRequired();
        entity.Property(photo => photo.ContentType).HasColumnName("content_type").HasMaxLength(100);
        entity.Property(photo => photo.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue((short)0)
            .IsRequired();
        entity.Property(photo => photo.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        entity.HasOne(photo => photo.Request)
            .WithMany(request => request.Photos)
            .HasForeignKey(photo => photo.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureServiceQuote(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ServiceQuote>();

        entity.ToTable("service_quotes");
        entity.HasKey(quote => quote.Id);
        entity.Property(quote => quote.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");
        entity.Property(quote => quote.RequestId)
            .HasColumnName("request_id")
            .HasColumnType("uuid");
        entity.Property(quote => quote.ProfessionalId)
            .HasColumnName("professional_id")
            .HasColumnType("uuid");
        entity.Property(quote => quote.AmountMxn)
            .HasColumnName("amount_mxn")
            .HasPrecision(10, 2)
            .IsRequired();
        entity.Property(quote => quote.Message).HasColumnType("text");
        entity.Property(quote => quote.EstimatedArrivalMinutes)
            .HasColumnName("estimated_arrival_minutes");
        entity.Property(quote => quote.Status)
            .HasDefaultValue(QuoteStatus.Sent)
            .IsRequired();
        entity.Property(quote => quote.ExpiresAt).HasColumnName("expires_at");
        ConfigureCreatedAndUpdated(entity, quote => quote.CreatedAt, quote => quote.UpdatedAt);

        entity.HasOne(quote => quote.Request)
            .WithMany(request => request.Quotes)
            .HasForeignKey(quote => quote.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(quote => quote.Professional)
            .WithMany(profile => profile.Quotes)
            .HasForeignKey(quote => quote.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureServiceRequestStatusHistory(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ServiceRequestStatusHistory>();

        entity.ToTable("service_request_status_history");
        entity.HasKey(history => history.Id);
        entity.Property(history => history.Id)
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd()
            .UseIdentityAlwaysColumn();
        entity.Property(history => history.RequestId)
            .HasColumnName("request_id")
            .HasColumnType("uuid");
        entity.Property(history => history.FromStatus).HasColumnName("from_status");
        entity.Property(history => history.ToStatus)
            .HasColumnName("to_status")
            .IsRequired();
        entity.Property(history => history.ChangedByUserId)
            .HasColumnName("changed_by_user_id")
            .HasColumnType("uuid");
        entity.Property(history => history.Note).HasColumnType("text");
        entity.Property(history => history.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        entity.HasOne(history => history.Request)
            .WithMany(request => request.StatusHistory)
            .HasForeignKey(history => history.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(history => history.ChangedByUser)
            .WithMany()
            .HasForeignKey(history => history.ChangedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }


    private static void ConfigureCreatedAndUpdated<TEntity>(
        EntityTypeBuilder<TEntity> entity,
        Expression<Func<TEntity, DateTimeOffset>> createdAt,
        Expression<Func<TEntity, DateTimeOffset>> updatedAt)
        where TEntity : class
    {
        entity.Property(createdAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();
        entity.Property(updatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("now()")
            .IsRequired();
    }
}
