using ArvidsonFoto.Areas.Identity.Data;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Extensions;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Security;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using Scalar.AspNetCore;
using IdentityContext = ArvidsonFoto.Areas.Identity.Data.ArvidsonFotoIdentityContext;

namespace ArvidsonFoto;

public class Program
{
    public static void Main(string[] args)
    {
        // Determine if we're in development mode early
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var isDevelopment = environment.Equals("Development", StringComparison.OrdinalIgnoreCase);

        // Build a temporary configuration to get logging settings
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Configure Serilog from appsettings with Console sink in Development
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration) // Read from appsettings.json
            .WriteTo.File("logs/appLog.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 150, retainedFileTimeLimit: TimeSpan.FromDays(90));

        // Add Console sink in Development for easier debugging
        if (isDevelopment)
        {
            _ = loggerConfig.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        Log.Logger = loggerConfig.CreateLogger();

        try
        {
            Log.Warning("Starting web application in {Environment} mode", environment);

            // Log configuration summary so key parameters are visible in the Aspire dashboard Structured Logs
            LogConfigurationSummary(configuration, environment, isDevelopment);

            var builder = WebApplication.CreateBuilder(args);

            // Add Aspire service defaults (observability, health checks, resilience, service discovery)
            _ = builder.AddServiceDefaults();

            // Expose important configuration values as OpenTelemetry resource attributes so they appear
            // as metadata on every trace and structured-log entry in the Aspire dashboard
            _ = builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddAttributes(
                    GetConfigurationAttributes(builder.Configuration)));

            // Add services to the container
            ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            ConfigureMiddleware(app, app.Environment, app.Configuration);

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        _ = services.AddDatabaseDeveloperPageExceptionFilter();

        // Configure SMTP settings from appsettings
        _ = services.Configure<Core.Configuration.SmtpSettings>(
            configuration.GetSection("SmtpSettings"));

        // Database configuration - ONLY Core DbContext now
        var useInMemoryDb = Environment.GetEnvironmentVariable("CODESPACES") != null ||
                           Environment.GetEnvironmentVariable("GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN") != null ||
                           configuration.GetConnectionString("UseInMemoryDatabase") == "true";

        if (useInMemoryDb)
        {
            // Use In-Memory database in Codespaces or when configured
            _ = services.AddDbContext<ArvidsonFotoCoreDbContext>(options =>
                options.UseInMemoryDatabase("ArvidsonFotoInMemory"));
            _ = services.AddDbContext<IdentityContext>(options =>
                options.UseInMemoryDatabase("ArvidsonFotoInMemory"));
        }
        else
        {
            // Use SQL Server locally
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _ = services.AddDbContext<ArvidsonFotoCoreDbContext>(options =>
                options.UseSqlServer(connectionString));
            _ = services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(connectionString));
        }

        // Identity configuration (moved from IdentityHostingStartup.cs)
        _ = services.AddDefaultIdentity<ArvidsonFotoUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // Enable passkey table storage (requires AspNetUserPasskeys migration)
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
            })
            .AddEntityFrameworkStores<IdentityContext>();

        // Passkey (WebAuthn) configuration
        _ = services.Configure<IdentityPasskeyOptions>(options =>
        {
            // ServerDomain defaults to the host header if not set.
            // In production, set this to your domain to prevent subdomain attacks.
            // options.ServerDomain = "arvidsonfoto.se";
            options.AuthenticatorTimeout = TimeSpan.FromMinutes(5);
        });

        // Add frontend services - all using Core now
        _ = services.AddScoped<IGuestBookService, GuestBookService>();
        _ = services.AddScoped<IPageCounterService, PageCounterService>();
        _ = services.AddScoped<IContactService, ContactService>();
        _ = services.AddScoped<IFacebookService, FacebookService>();

        // Add HttpClient for FacebookService
        _ = services.AddHttpClient<IFacebookService, FacebookService>();

        // Add backend API services
        _ = services.AddScoped<IApiCategoryService, ApiCategoryService>();
        _ = services.AddScoped<IApiImageService, ApiImageService>();

        // CORS configuration for API calls from localhost and production
        var corsPolicyName = "ArvidsonFotoCorsPolicy";
        _ = services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName, policy =>
            {
                _ = policy.WithOrigins(
                        "https://localhost:5001",
                        "http://localhost:5000",
                        "https://www.arvidsonfoto.se",
                        "https://arvidsonfoto.se"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        _ = services.AddControllersWithViews().AddDataAnnotationsLocalization(options =>
            options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(ValidationResource)));
        _ = services.AddRazorPages();
        _ = services.AddLocalization(options => options.ResourcesPath = "Resources");
        _ = services.Configure<RequestLocalizationOptions>(options =>
        {
            _ = options.SetDefaultCulture("sv-SE")
                .AddSupportedCultures("sv-SE", "en-US")
                .AddSupportedUICultures("sv-SE", "en-US");
            options.RequestCultureProviders.Insert(1, new CustomRequestCultureProvider(context =>
            {
                var path = context.Request.Path.Value;
                if (LocalizedRoutes.IsSwedishApiPath(path))
                {
                    return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult("sv-SE"));
                }

                return Task.FromResult(
                    LocalizedRoutes.IsEnglishPath(path) ? new ProviderCultureResult("en-US") : null);
            }));
        });

        // ===== ROUTING CONFIGURATION =====
        // Enable case-insensitive and URL-decoding routing
        _ = services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = false; // Keep original casing in generated URLs
            options.LowercaseQueryStrings = false;
        });

        // OpenAPI configuration for API documentation (using .NET 10 built-in support)
        _ = services.AddOpenApi();

        // Configure JavaScript engine for SCSS compilation
        _ = services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName)
                .AddV8();

        // WebOptimizer for SCSS compilation and CSS/JS minification
        _ = services.AddWebOptimizer(pipeline =>
        {
            // Compile SCSS to CSS and minify
            _ = pipeline.AddScssBundle("/css/site.css", "wwwroot/css/site.scss")
                .UseContentRoot();

            // Minify JavaScript files
            _ = pipeline.AddJavaScriptBundle("/js/site.min.js", "wwwroot/js/site.js")
                .UseContentRoot();

            _ = pipeline.AddJavaScriptBundle("/js/contactform.min.js", "wwwroot/js/ContactForm.js")
                .UseContentRoot();

            _ = pipeline.AddJavaScriptBundle("/js/glightbox.min.js", "wwwroot/js/gLightBoxOptions.js")
                .UseContentRoot();

            _ = pipeline.AddJavaScriptBundle("/js/passkey.min.js", "wwwroot/js/passkey.js")
                .UseContentRoot();
        });
    }

    private static void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env, IConfiguration configuration)
    {
        // Seed in-memory database if using in-memory database
        var useInMemoryDb = Environment.GetEnvironmentVariable("CODESPACES") != null ||
                           Environment.GetEnvironmentVariable("GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN") != null ||
                           configuration.GetConnectionString("UseInMemoryDatabase") == "true";

        if (useInMemoryDb)
        {
            using var scope = app.Services.CreateScope();
            var coreContext = scope.ServiceProvider.GetRequiredService<ArvidsonFotoCoreDbContext>();
            _ = coreContext.Database.EnsureCreated();

            // Seeda data om databasen är tom
            if (!coreContext.TblImages.Any())
            {
                Log.Information("Seeding in-memory database with test data...");
                coreContext.SeedInMemoryDatabase();
                Log.Information("In-memory database seeded successfully with {ImageCount} images, {CategoryCount} categories, {GuestbookCount} guestbook entries",
                    coreContext.TblImages.Count(),
                    coreContext.TblMenus.Count(),
                    coreContext.TblGbs.Count());
            }
            else
            {
                Log.Information("In-memory database already contains data - skipping seed");
            }
        }

        // ===== EAGER LOAD CATEGORY CACHE AT STARTUP =====
        // Pre-load all categories into cache to improve performance
        // This reduces database queries from ~350k to <1k per 5 minutes
        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var categoryService = scope.ServiceProvider.GetRequiredService<IApiCategoryService>();

                Log.Debug("Pre-loading category cache...");
                var startTime = DateTime.UtcNow;

                // Load all categories - this will cache them with GetAll()
                var allCategories = categoryService.GetAll();

                // Pre-cache all category names and paths for bulk operations
                var allCategoryIds = allCategories
                    .Where(c => c.CategoryId.HasValue)
                    .Select(c => c.CategoryId!.Value)
                    .ToList();

                if (allCategoryIds.Count != 0)
                {
                    _ = categoryService.GetCategoryNamesBulk(allCategoryIds);
                    _ = categoryService.GetCategoryPathsBulk(allCategoryIds);
                }

                var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
                Log.Debug("Category cache pre-loaded successfully in {ElapsedMs}ms with {Count} categories",
                    elapsed, allCategories.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to pre-load category cache - will load on demand");
            }
        }

        if (env.IsDevelopment())
        {
            _ = app.UseDeveloperExceptionPage();
            _ = app.UseMigrationsEndPoint();
            _ = app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
        }
        else
        {
            _ = app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = app.UseHsts();
        }

        // Only use HTTPS redirection in production, not in development/Codespaces
        if (!env.IsDevelopment())
        {
            _ = app.UseHttpsRedirection();
        }

        // WebOptimizer middleware - add before UseStaticFiles
        _ = app.UseWebOptimizer();

        _ = app.UseStaticFiles();

        _ = app.UseRequestLocalization();
        _ = app.Use(async (context, next) =>
        {
            var culture = context.Request.Query["culture"].ToString();
            if (culture.Length == 0 && LocalizedRoutes.IsEnglishPath(context.Request.Path.Value))
            {
                culture = "en-US";
            }

            if (culture is "sv-SE" or "en-US")
            {
                context.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = context.Request.IsHttps,
                        SameSite = SameSiteMode.Lax,
                        IsEssential = true,
                        Expires = DateTimeOffset.UtcNow.AddYears(1)
                    });
            }
            await next();
        });

        // Add input validation middleware to prevent SQL injection and malicious input
        _ = app.UseMiddleware<InputValidationMiddleware>();

        _ = app.UseRouting();

        // CORS middleware - must be after UseRouting and before UseAuthorization
        _ = app.UseCors("ArvidsonFotoCorsPolicy");

        _ = app.UseAuthentication();
        _ = app.UseAuthorization();

        // Map Aspire default endpoints (health checks, alive checks)
        _ = app.MapDefaultEndpoints();

        foreach (var (path, controller, action, sortOrder) in new (string Path, string Controller, string Action, string? SortOrder)[]
        {
            ("information", "Info", "Index", null),
            ("information/buy-photos", "Info", "Kop_av_bilder", null),
            ("information/guestbook", "Info", "Gastbok", null),
            ("information/contact", "Info", "Kontakta", null),
            ("information/about", "Info", "Om_mig", null),
            ("information/sitemap", "Info", "Sidkarta", null),
            ("information/copyright", "Info", "Copyright", null)
        })
        {
            app.MapControllerRoute($"english-{path}", path,
                new { controller, action, sortOrder })
                .Add(endpoint => endpoint.Metadata.Add(new SuppressLinkGenerationMetadata()));
        }
        _ = app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        _ = app.MapRazorPages();

        // .NET 10: endpoint-based static asset serving with fingerprinting and compression
        _ = app.MapStaticAssets();

        // Passkey (WebAuthn) endpoints (excluded from OpenAPI/Scalar). Used by passkey.js.
        _ = app.MapPasskeyEndpoints();

        // OpenAPI endpoints - only in development
        if (env.IsDevelopment())
        {
            // Native .NET 10 OpenAPI JSON endpoint
            _ = app.MapOpenApi();

            // Scalar UI for interactive API documentation  
            _ = app.MapScalarApiReference(options =>
            {
                _ = options
                    .WithTitle("ArvidsonFoto API")
                    .WithTheme(ScalarTheme.Purple)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });

            Log.Information("OpenAPI documentation available at: /scalar/v1");
            Log.Information("OpenAPI JSON schema available at: /openapi/v1.json");
        }
    }

    /// <summary>
    /// Logs a structured configuration summary at startup so it is visible in the
    /// Aspire dashboard's Structured Logs view for each service instance.
    /// </summary>
    private static void LogConfigurationSummary(IConfiguration configuration, string environment, bool isDevelopment)
    {
        var useInMemoryDb = IsUsingInMemoryDatabase(configuration);

        var dbType = useInMemoryDb ? "InMemory" : "SqlServer";
        var connectionInfo = useInMemoryDb
            ? "InMemory"
            : SanitizeConnectionString(configuration.GetConnectionString("DefaultConnection"));

        var serilogMinLevel = configuration["Serilog:MinimumLevel:Default"] ?? "Information";
        var logFilePath = configuration["Serilog:WriteTo:0:Args:path"] ?? "logs/appLog.txt";

        var smtpServer = configuration["SmtpSettings:Server"];
        var smtpPort = configuration["SmtpSettings:Port"] ?? "587";
        var smtpConfigured = !string.IsNullOrWhiteSpace(smtpServer);

        Log.Information(
            "Configuration: Environment={Environment}, DatabaseType={DatabaseType}, " +
            "DatabaseConnection={DatabaseConnection}, LogMinLevel={LogMinLevel}, " +
            "LogFilePath={LogFilePath}, ConsoleLogging={ConsoleLogging}, " +
            "SmtpConfigured={SmtpConfigured}, SmtpServer={SmtpServer}, SmtpPort={SmtpPort}",
            environment,
            dbType,
            connectionInfo,
            serilogMinLevel,
            logFilePath,
            isDevelopment,
            smtpConfigured,
            smtpConfigured ? smtpServer : "(not configured)",
            smtpPort);
    }

    /// <summary>
    /// Returns OpenTelemetry resource attributes that represent key configuration values.
    /// These attributes are attached to every trace and structured-log entry exported via OTLP,
    /// making them visible as metadata in the Aspire dashboard.
    /// </summary>
    private static IEnumerable<KeyValuePair<string, object>> GetConfigurationAttributes(IConfiguration configuration)
    {
        var useInMemoryDb = IsUsingInMemoryDatabase(configuration);

        var dbType = useInMemoryDb ? "InMemory" : "SqlServer";
        var serilogMinLevel = configuration["Serilog:MinimumLevel:Default"] ?? "Information";
        var logFilePath = configuration["Serilog:WriteTo:0:Args:path"] ?? "logs/appLog.txt";
        var smtpServer = configuration["SmtpSettings:Server"] ?? string.Empty;
        var smtpPort = configuration["SmtpSettings:Port"] ?? "587";

        return
        [
            new KeyValuePair<string, object>("app.config.database_type", dbType),
            new KeyValuePair<string, object>("app.config.log_min_level", serilogMinLevel),
            new KeyValuePair<string, object>("app.config.log_file", logFilePath),
            new KeyValuePair<string, object>("app.config.smtp_server", string.IsNullOrEmpty(smtpServer) ? "(not configured)" : smtpServer),
            new KeyValuePair<string, object>("app.config.smtp_port", smtpPort),
        ];
    }

    /// <summary>
    /// Returns <c>true</c> when the application is configured to use an in-memory database,
    /// either via Codespaces environment variables or the <c>UseInMemoryDatabase</c> connection string.
    /// </summary>
    private static bool IsUsingInMemoryDatabase(IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("CODESPACES") != null ||
        Environment.GetEnvironmentVariable("GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN") != null ||
        configuration.GetConnectionString("UseInMemoryDatabase") == "true";
    }

    /// <summary>
    /// Returns a sanitized connection string safe for logging by replacing any password value
    /// with "***" so credentials are never written to logs or the Aspire dashboard.
    /// </summary>
    private static string SanitizeConnectionString(string? connectionString)
    {
        return string.IsNullOrEmpty(connectionString)
            ? "(not configured)"
            : System.Text.RegularExpressions.Regex.Replace(
            connectionString,
            @"(?i)(password|pwd)\s*=\s*[^;]+",
            "$1=***");
    }
}