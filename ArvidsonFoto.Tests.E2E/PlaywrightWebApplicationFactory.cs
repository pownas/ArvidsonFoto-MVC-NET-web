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
/// Implements <see cref="IAsyncLifetime"/> so xUnit calls
/// <see cref="InitializeAsync"/> on the fixture before any test class instance
/// is created.  The host is therefore fully bound before the first test runs.
///
/// A free TCP port is chosen before Kestrel starts by briefly binding a
/// <see cref="TcpListener"/> to port 0 and reading back the OS-assigned port.
/// The concrete port number is then passed to <c>UseUrls</c> inside
/// <see cref="CreateHost"/> — not in <see cref="ConfigureWebHost"/> — so that
/// Kestrel is registered on the DI container <em>after</em> the in-memory
/// <c>TestServer</c> that <c>WebApplicationFactory</c> registers first.
/// Because DI resolves to the <em>last</em> registered <c>IServer</c>,
/// Kestrel wins and a real TCP socket is opened on
/// <see cref="ServerAddress"/>.
/// </summary>
public class PlaywrightWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _serverAddress;

    public PlaywrightWebApplicationFactory()
    {
        _serverAddress = $"http://localhost:{GetFreePort()}";
    }

    /// <summary>
    /// The HTTP base address that the test server is listening on.
    /// Available immediately after <see cref="InitializeAsync"/> completes.
    /// </summary>
    public string ServerAddress => _serverAddress;

    // -------------------------------------------------------------------------
    // IAsyncLifetime — called by xUnit before the fixture is injected into
    // any test class constructor.
    // -------------------------------------------------------------------------

    /// <summary>Triggers lazy host creation, binding Kestrel to the port.</summary>
    public Task InitializeAsync()
    {
        // Accessing Services triggers WebApplicationFactory.EnsureServer(),
        // which calls CreateHost() and starts Kestrel synchronously.
        _ = Services;
        return Task.CompletedTask;
    }

    /// <summary>Delegates to WebApplicationFactory's own async disposal.</summary>
    Task IAsyncLifetime.DisposeAsync() => base.DisposeAsync().AsTask();

    // -------------------------------------------------------------------------
    // WebApplicationFactory overrides
    // -------------------------------------------------------------------------

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

        // NOTE: UseKestrel().UseUrls() is intentionally NOT called here.
        // WebApplicationFactory registers TestServer via UseTestServer() on
        // the same IWebHostBuilder wrapper.  Any UseKestrel() call here would
        // be overridden because the wrapper does not guarantee ordering with
        // respect to the factory's own UseTestServer() call.
        // See CreateHost() below where Kestrel is registered last and wins.
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Register Kestrel here, on the raw IHostBuilder, AFTER
        // WebApplicationFactory has already registered TestServer via its
        // internal ConfigureWebHost call.  DI resolves IServer to the last
        // registered implementation — so Kestrel (registered last) wins and
        // opens a real TCP socket on _serverAddress.
        builder.ConfigureWebHost(webBuilder =>
            webBuilder.UseKestrel().UseUrls(_serverAddress));

        // Build and start the host with Kestrel as IServer.
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
        // for non-Playwright HTTP assertions if needed.
        ClientOptions.BaseAddress = new Uri(_serverAddress);

        return host;
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

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
