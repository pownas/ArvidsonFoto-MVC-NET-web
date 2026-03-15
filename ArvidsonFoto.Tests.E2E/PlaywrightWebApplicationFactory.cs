using ArvidsonFoto.Core.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ArvidsonFoto.Tests.E2E;

/// <summary>
/// Custom WebApplicationFactory that starts a real Kestrel HTTP server so that
/// Playwright can connect via HTTP without requiring the application to be
/// started manually before running the tests.
///
/// The recommended .NET 10 pattern (see
/// https://learn.microsoft.com/aspnet/core/test/integration-tests) is to
/// override <see cref="CreateHost"/> so that Kestrel is used in place of the
/// default in-memory TestServer.  Port 0 lets the OS assign a free port, and
/// <see cref="ServerAddress"/> exposes the resulting URL to each test.
/// </summary>
public class PlaywrightWebApplicationFactory : WebApplicationFactory<Program>
{
    private string _serverAddress = string.Empty;

    /// <summary>
    /// The HTTP base address that the test server is listening on.
    /// Available once the factory has been initialised (i.e. after
    /// <see cref="EnsureStarted"/> has been called).
    /// </summary>
    public string ServerAddress => _serverAddress;

    /// <summary>
    /// Triggers lazy host creation so that <see cref="ServerAddress"/> is
    /// populated before the first test navigates to a URL.
    /// Call this once at the start of each test class.
    /// </summary>
    public void EnsureStarted() => _ = Services;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use an in-memory database – no SQL Server instance required.
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:UseInMemoryDatabase"] = "true",
                // Provide stub SMTP settings so the contact-form controller
                // can initialise without a real mail server.
                ["SmtpSettings:Server"] = "smtp.test.local",
                ["SmtpSettings:Port"] = "587",
                ["SmtpSettings:SenderEmail"] = "test@test.local",
                ["SmtpSettings:SenderPassword"] = "test-password",
                ["SmtpSettings:EnableSsl"] = "true"
            });
        });

        // Use Kestrel with a random available port so the test server never
        // conflicts with the developer's locally running instance on port 5001.
        builder.UseKestrel().UseUrls("http://localhost:0");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Build and start the Kestrel host.  We do NOT call base.CreateHost()
        // because that would start the in-memory TestServer instead of Kestrel.
        var host = builder.Build();
        host.Start();

        // Seed the in-memory database with representative test data.
        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<ArvidsonFotoCoreDbContext>();
            if (db != null)
            {
                db.Database.EnsureCreated();
                if (!db.TblImages.Any())
                {
                    db.SeedInMemoryDatabase();
                }
            }
        }

        // Discover the port that Kestrel bound to and expose it as a URL.
        var server = host.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>();
        if (addresses?.Addresses.Count > 0)
        {
            _serverAddress = addresses.Addresses.First();
        }

        // Store the address on ClientOptions so callers can use CreateClient()
        // for non-Playwright assertions if needed.
        ClientOptions.BaseAddress = new Uri(_serverAddress);

        return host;
    }
}
