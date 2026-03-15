using System.Net;
using System.Net.Sockets;
using ArvidsonFoto.Core.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ArvidsonFoto.Tests.E2E;

/// <summary>
/// Custom WebApplicationFactory that starts a real Kestrel HTTP server so that
/// Playwright can connect via HTTP without requiring the application to be
/// started manually before running the tests.
///
/// A free TCP port is chosen before Kestrel starts by briefly binding a
/// <see cref="TcpListener"/> to port 0 and reading back the OS-assigned port.
/// The concrete port number is then passed to <c>UseUrls</c>, so
/// <see cref="ServerAddress"/> always contains a usable URL rather than the
/// placeholder <c>http://localhost:0</c> that Kestrel leaves in
/// <c>IServerAddressesFeature.Addresses</c> when port 0 is used directly.
/// </summary>
public class PlaywrightWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _serverAddress;

    public PlaywrightWebApplicationFactory()
    {
        _serverAddress = $"http://localhost:{GetFreePort()}";
    }

    /// <summary>
    /// The HTTP base address that the test server is listening on.
    /// Available as soon as the factory is constructed.
    /// </summary>
    public string ServerAddress => _serverAddress;

    /// <summary>
    /// Triggers lazy host creation so that Kestrel is bound before the first
    /// test navigates to a URL.  Call this once in <c>InitializeAsync</c>.
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

        // Bind Kestrel to the pre-selected concrete port so that
        // ServerAddress is always a valid, non-zero URL.
        builder.UseKestrel().UseUrls(_serverAddress);
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

        // Store the address on ClientOptions so callers can use CreateClient()
        // for non-Playwright assertions if needed.
        ClientOptions.BaseAddress = new Uri(_serverAddress);

        return host;
    }

    /// <summary>
    /// Finds a free TCP port on the loopback interface by briefly binding a
    /// listener to port 0 and reading back the OS-assigned port number.
    /// </summary>
    private static int GetFreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
