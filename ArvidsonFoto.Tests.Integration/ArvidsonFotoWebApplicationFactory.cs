using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Tests.Integration;

/// <summary>
/// Custom WebApplicationFactory for integration testing.
/// Configures the test server to use an in-memory database with Core models.
/// </summary>
public class ArvidsonFotoWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ArvidsonFotoCoreDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext using an in-memory database for testing
            services.AddDbContext<ArvidsonFotoCoreDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryTestDb");
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ArvidsonFotoCoreDbContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();

                // Seed the database with test data if needed
                SeedTestData(db);
            }
        });
    }

    private static void SeedTestData(ArvidsonFotoCoreDbContext context)
    {
        // Add any test data needed for integration tests
        // This data will be available to all tests
        
        // Example: Seed some categories using Core models
        if (!context.TblMenus.Any())
        {
            context.TblMenus.AddRange(
                new TblMenu
                {
                    MenuCategoryId = 1,
                    MenuParentCategoryId = null,
                    MenuDisplayName = "Fåglar",
                    MenuUrlSegment = "faglar"
                },
                new TblMenu
                {
                    MenuCategoryId = 2,
                    MenuParentCategoryId = 1,
                    MenuDisplayName = "Tättingar",
                    MenuUrlSegment = "tattingar"
                }
            );
            context.SaveChanges();
        }
    }
}
