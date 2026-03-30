using System.Net;
using System.Net.Sockets;

namespace ArvidsonFoto.Tests.E2E;

/// <summary>
/// Self-hosted Kestrel server fixture used by Playwright E2E tests.
///
/// Builds and starts a real <see cref="WebApplication"/> on a randomly-chosen
/// free port, using exactly the same service and middleware configuration as
/// <c>Program.cs</c> (<see cref="Program.ConfigureServices"/> and
/// <see cref="Program.ConfigureMiddleware"/>).  An in-memory database and
/// stub SMTP settings are injected so no external infrastructure is required.
///
/// Implements <see cref="IAsyncLifetime"/> so xUnit starts the server before
/// any <c>IClassFixture</c>-consuming test class is constructed, and stops it
/// cleanly after all tests in the class finish.
///
/// The previous approach (inheriting from
/// <see cref="Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory{TEntryPoint}"/>
/// and overriding <c>CreateHost</c> to add Kestrel) caused an
/// <see cref="InvalidCastException"/> because <c>WebApplicationFactory</c>
/// internally casts <c>IServer</c> to <c>TestServer</c> after
/// <c>CreateHost</c> returns — an invariant that cannot be avoided while
/// inheriting from that class.
/// </summary>
public class PlaywrightWebApplicationFactory : IAsyncLifetime
{
    private WebApplication? _app;
    private readonly string _serverAddress;

    public PlaywrightWebApplicationFactory()
    {
        _serverAddress = $"http://localhost:{GetFreePort()}";
    }

    /// <summary>The HTTP base address Kestrel is listening on.</summary>
    public string ServerAddress => _serverAddress;

    // -------------------------------------------------------------------------
    // IAsyncLifetime
    // -------------------------------------------------------------------------

    public async Task InitializeAsync()
    {
        // Locate the content root so that WebOptimizer can find wwwroot assets.
        // typeof(Program).Assembly.Location resolves to the ArvidsonFoto.dll
        // that was copied to this test project's output directory.
        var contentRoot = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Development",
            ContentRootPath = contentRoot,
        });

        // Inject test-specific configuration on top of any appsettings files.
        // AddInMemoryCollection is added last and therefore takes highest precedence.
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:UseInMemoryDatabase"] = "true",
            // Provide stub SMTP settings so the contact-form controller
            // can initialise without a real mail server.
            ["SmtpSettings:Server"]         = "smtp.test.local",
            ["SmtpSettings:Port"]           = "587",
            ["SmtpSettings:SenderEmail"]    = "test@test.local",
            ["SmtpSettings:SenderPassword"] = "test-password",
            ["SmtpSettings:EnableSsl"]      = "true",
        });

        // MapHealthChecks (called by ConfigureMiddleware → MapDefaultEndpoints in
        // Development mode) requires HealthCheckService to be registered.
        // AddServiceDefaults() is only called in Program.Main, so we register
        // the bare minimum here to satisfy that requirement.
        builder.Services.AddHealthChecks();

        // Apply the same service registrations as the real application.
        Program.ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

        // Bind Kestrel to the pre-selected concrete port.
        builder.WebHost.UseUrls(_serverAddress);

        _app = builder.Build();

        // Apply the same middleware pipeline as the real application.
        // ConfigureMiddleware also seeds the in-memory database because
        // "ConnectionStrings:UseInMemoryDatabase" is "true".
        Program.ConfigureMiddleware(_app, _app.Environment, _app.Configuration);

        await _app.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
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
