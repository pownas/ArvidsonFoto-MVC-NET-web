using ArvidsonFoto.Core.Data;

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

                // Seed the database with test data using the standard seeder
                db.SeedInMemoryDatabase();
            }
        });
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
