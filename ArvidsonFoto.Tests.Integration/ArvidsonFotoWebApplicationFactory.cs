using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ArvidsonFoto.Data;

namespace ArvidsonFoto.Tests.Integration;

/// <summary>
/// Custom WebApplicationFactory for integration testing.
/// Configures the test server to use an in-memory database.
/// </summary>
public class ArvidsonFotoWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ArvidsonFotoDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext using an in-memory database for testing
            services.AddDbContext<ArvidsonFotoDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryTestDb");
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ArvidsonFotoDbContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();

                // Seed the database with test data if needed
                SeedTestData(db);
            }
        });
    }

    private static void SeedTestData(ArvidsonFotoDbContext context)
    {
        // Add any test data needed for integration tests
        // This data will be available to all tests
        
        // Example: Seed some categories
        if (!context.TblMenus.Any())
        {
            context.TblMenus.AddRange(
                new Models.TblMenu
                {
                    MenuId = 1,
                    MenuMainId = null,
                    MenuText = "Fåglar",
                    MenuUrltext = "Faglar"
                },
                new Models.TblMenu
                {
                    MenuId = 2,
                    MenuMainId = 1,
                    MenuText = "Tättingar",
                    MenuUrltext = "Tattingar"
                }
            );
            context.SaveChanges();
        }
    }
}
