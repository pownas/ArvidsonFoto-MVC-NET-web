using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ArvidsonFoto.Core.Data;

/// <summary>
/// Design-time factory for ArvidsonFotoCoreDbContext.
/// This enables EF Core tools to create a DbContext instance at design time
/// without requiring a running application.
/// </summary>
public class ArvidsonFotoCoreDbContextFactory : IDesignTimeDbContextFactory<ArvidsonFotoCoreDbContext>
{
    /// <summary>
    /// Skapar en instans av ArvidsonFotoCoreDbContext för design-time operationer.
    /// </summary>
    /// <param name="args">Kommandoradsargument som kan innehålla anslutningssträngar</param>
    /// <returns>En konfigurerad ArvidsonFotoCoreDbContext-instans</returns>
    public ArvidsonFotoCoreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ArvidsonFotoCoreDbContext>();

        // Use a default connection string for design-time operations
        // This can be overridden by setting the ASPNETCORE_ENVIRONMENT variable
        // or by providing connection string through command line arguments
        var connectionString = GetConnectionString(args);

        optionsBuilder.UseSqlServer(connectionString);

        return new ArvidsonFotoCoreDbContext(optionsBuilder.Options);
    }

    private static string GetConnectionString(string[] args)
    {
        // Check if connection string is provided via command line arguments
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--connection" || args[i] == "-c")
            {
                return args[i + 1];
            }
        }

        // Check environment variable
        var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            return connectionString;
        }

        // Default connection string for design-time operations
        // This should match your typical development database setup
        return "Server=(localdb)\\mssqllocaldb;Database=ArvidsonFoto;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";
    }
}
