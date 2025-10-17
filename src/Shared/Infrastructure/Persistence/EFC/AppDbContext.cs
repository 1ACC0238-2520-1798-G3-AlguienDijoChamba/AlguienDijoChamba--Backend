using System.Reflection;
using AlguienDijoChamba.Api.IAM.Domain;
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using AlguienDijoChamba.Api.Jobs.Domain;
using Microsoft.EntityFrameworkCore;

namespace AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<JobRequest> JobRequests { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Professional> Professionals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}