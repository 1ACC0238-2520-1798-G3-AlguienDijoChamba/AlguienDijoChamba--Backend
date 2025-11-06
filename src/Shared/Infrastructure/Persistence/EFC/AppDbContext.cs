using System.Reflection;
using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.IAM.Domain;
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Jobs.Domain;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Llama al método base PRIMERO. Esto aplica convenciones predeterminadas.
        base.OnModelCreating(modelBuilder);
        
        // 2. Aplica configuraciones externas (IEntityTypeConfiguration).
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 3. Aplica las configuraciones manuales para sobrescribir convenciones o configuraciones externas.
        
        modelBuilder.Entity<ProfessionalTag>(entity =>
        {
            // 1. Clave Compuesta (Resuelve el error de la clave primaria)
            entity.HasKey(pt => new { pt.ProfessionalId, pt.TagId }); 
        
            // 2. 🚀 CORRECCIÓN CLAVE: Definición explícita de la relación Tag
            // Esto le dice a EF Core: "Usa la propiedad de navegación 'Tag' y vincúlala a la columna 'TagId'."
            entity.HasOne(pt => pt.Tag) // Navega de ProfessionalTag A Tag
                .WithMany(t => t.ProfessionalTags) // Un Tag tiene muchos ProfessionalTags
                .HasForeignKey(pt => pt.TagId) // Usa la columna TagId como FK
                .OnDelete(DeleteBehavior.Cascade); // Opcional: Elimina las uniones si se elimina el Tag

            // 3. Definición explícita de la relación Professional
            // Aunque Professional está en otro dominio, lo configuramos para claridad.
            entity.HasOne<Professional>()
                .WithMany(p => p.ProfessionalTags)
                .HasForeignKey(pt => pt.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade); // Opcional
        });

        // 🔒 Unicidad del nombre del Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasIndex(t => t.Name).IsUnique();
        });
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}