using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Use a dummy connection string for design-time operations
        // The actual connection string will be provided at runtime
        optionsBuilder.UseMySql(
            "Server=localhost;Database=alguiendijochamba;User=root;Password=password;",
            new MySqlServerVersion(new Version(8, 0, 21))
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}
