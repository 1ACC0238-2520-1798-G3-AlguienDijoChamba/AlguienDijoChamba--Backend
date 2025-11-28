using System.Reflection;
using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.IAM.Domain;
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Jobs.Domain;
using AlguienDijoChamba.Api.Notifications.Domain;
using Microsoft.EntityFrameworkCore;

namespace AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<JobRequest> JobRequests { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Professional> Professionals { get; set; }
    public DbSet<Customer> Customers { get; set; } 
    public DbSet<UserReputationTechnician> Reputations { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ProfessionalTag> ProfessionalTags { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Llama al método base PRIMERO.
        base.OnModelCreating(modelBuilder);

        // 2. Aplica configuraciones externas
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 3. Configuraciones manuales

        modelBuilder.Entity<ProfessionalTag>(entity =>
        {
            entity.HasKey(pt => new { pt.ProfessionalId, pt.TagId });

            entity.HasOne(pt => pt.Tag)
                .WithMany(t => t.ProfessionalTags)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Professional>()
                .WithMany(p => p.ProfessionalTags)
                .HasForeignKey(pt => pt.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasIndex(t => t.Name).IsUnique();
        });

        // ✨ RELACIÓN JobRequest → Professional
        modelBuilder.Entity<JobRequest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.Specialty).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.ScheduledHour).HasMaxLength(50);
            entity.Property(e => e.AdditionalMessage).HasMaxLength(500);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.TotalCost).HasPrecision(10, 2);

            entity.Property(e => e.Categories)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            entity.HasIndex(e => new { e.ClientId, e.Status });

            // 👇 FK hacia Professional (usa Professional.JobRequests como colección inversa)
            entity.HasOne(j => j.Professional)
                .WithMany(p => p.JobRequests)
                .HasForeignKey(j => j.ProfessionalId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
