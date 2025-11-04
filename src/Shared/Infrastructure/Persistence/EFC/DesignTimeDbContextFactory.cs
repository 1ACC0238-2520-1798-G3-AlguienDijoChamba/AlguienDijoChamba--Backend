using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Use connection string from environment variable or default for design-time operations
        // The actual connection string will be provided at runtime
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
            ?? "Server=localhost;Database=alguiendijochamba;User=root;Password=;";
        
        optionsBuilder.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 21))
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}
