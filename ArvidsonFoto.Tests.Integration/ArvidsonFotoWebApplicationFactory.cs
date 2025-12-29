using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Tests.Integration;

/// <summary>
/// Custom WebApplicationFactory for integration testing.
/// Configures the test server to use an in-memory database with Core models.
/// </summary>
public class ArvidsonFotoWebApplicationFactory : WebApplicationFactory<Program>
{
    public ArvidsonFotoWebApplicationFactory()
    {
        // Set environment variable to force in-memory database
        Environment.SetEnvironmentVariable("UseInMemoryDatabase", "true");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set configuration to use in-memory database
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:UseInMemoryDatabase"] = "true",
                ["SmtpSettings:Server"] = "smtp.test.com",
                ["SmtpSettings:Port"] = "587",
                ["SmtpSettings:SenderEmail"] = "test@test.com",
                ["SmtpSettings:SenderPassword"] = "test-password",
                ["SmtpSettings:EnableSsl"] = "true"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Build the service provider to seed data
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
        // Only add data if it doesn't exist
        if (context.TblMenus.Any())
        {
            return; // Already seeded
        }
        
        // Add test data needed for integration tests
        
        // Seed some categories using Core models
        context.TblMenus.AddRange(
            new TblMenu
            {
                MenuCategoryId = 1,
                MenuParentCategoryId = null,
                MenuDisplayName = "Fåglar",
                MenuUrlSegment = "faglar",
                MenuDateUpdated = DateTime.Now
            },
            new TblMenu
            {
                MenuCategoryId = 2,
                MenuParentCategoryId = 1,
                MenuDisplayName = "Tättingar",
                MenuUrlSegment = "tattingar",
                MenuDateUpdated = DateTime.Now
            }
        );

        // Seed some test guestbook entries
        context.TblGbs.AddRange(
            new TblGb
            {
                GbId = 1,
                GbName = "Test User",
                GbEmail = "test@example.com",
                GbText = "Test guestbook entry",
                GbDate = DateTime.Now,
                GbReadPost = false
            }
        );

        context.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Clean up environment variable
            Environment.SetEnvironmentVariable("UseInMemoryDatabase", null);
        }
        base.Dispose(disposing);
    }
}
