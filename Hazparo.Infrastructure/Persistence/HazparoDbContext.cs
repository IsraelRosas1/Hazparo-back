using Microsoft.EntityFrameworkCore;
using Hazparo.Domain.Entities;

namespace Hazparo.Infrastructure.Persistence;

public class HazparoDbContext(DbContextOptions<HazparoDbContext> options): DbContext(options)
{
    internal DbSet<Customer> Customers {get;set;}
    internal DbSet<Professional> Professionals {get;set;}
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
 
    protected override void OnModelCreating(ModelBuilder modelBuilder )
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>()
        .OwnsOne(c=>c.HomeAddress);


        //Professional entity configuration
        modelBuilder.Entity<Professional>(entity =>
        {
            entity.OwnsOne(p => p.HomeAddress);

            entity.HasMany(p => p.Jobs)
                .WithOne(j => j.Professional)
                .HasForeignKey(j => j.ProfessionalId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasMany(p => p.Specialties)
                .WithMany(s => s.Professionals)
                .UsingEntity(j => j.ToTable("ProfessionalSpecialties"));
        });

        //Job entity configuration
        modelBuilder.Entity<Job> (entity =>
        {
           entity.OwnsOne(j => j.Address);

           entity.Property(j => j.QuotedPrice).HasPrecision(18, 2);
           entity.Property(j => j.FinalPrice).HasPrecision(18, 2);

           entity.HasOne(j => j.Customer)
                .WithMany()
                .HasForeignKey( r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }
}