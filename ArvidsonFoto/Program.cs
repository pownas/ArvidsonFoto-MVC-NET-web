using Microsoft.EntityFrameworkCore;
using JavaScriptEngineSwitcher.V8;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Services;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Security;
using ArvidsonFoto.Areas.Identity.Data;
using IdentityContext = ArvidsonFoto.Areas.Identity.Data.ArvidsonFotoIdentityContext;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Identity;
using OpenTelemetry;
using OpenTelemetry.Resources;

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
            loggerConfig.WriteTo.Console(
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
            builder.AddServiceDefaults();

            // Expose important configuration values as OpenTelemetry resource attributes so they appear
            // as metadata on every trace and structured-log entry in the Aspire dashboard
            builder.Services.AddOpenTelemetry()
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

    internal static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        // Configure SMTP settings from appsettings
        services.Configure<ArvidsonFoto.Core.Configuration.SmtpSettings>(
            configuration.GetSection("SmtpSettings"));

        // Database configuration - ONLY Core DbContext now
        var useInMemoryDb = Environment.GetEnvironmentVariable("CODESPACES") != null || 
                           Environment.GetEnvironmentVariable("GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN") != null ||
                           configuration.GetConnectionString("UseInMemoryDatabase") == "true";

        if (useInMemoryDb)
        {
            // Use In-Memory database in Codespaces or when configured
            services.AddDbContext<ArvidsonFotoCoreDbContext>(options =>
                options.UseInMemoryDatabase("ArvidsonFotoInMemory"));
            services.AddDbContext<IdentityContext>(options =>
                options.UseInMemoryDatabase("ArvidsonFotoInMemory"));
        }
        else
        {
            // Use SQL Server locally
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ArvidsonFotoCoreDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(connectionString));
        }

        // Identity configuration (moved from IdentityHostingStartup.cs)
        services.AddDefaultIdentity<ArvidsonFotoUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // Enable passkey table storage (requires AspNetUserPasskeys migration)
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
            })
            .AddEntityFrameworkStores<IdentityContext>();

        // Passkey (WebAuthn) configuration
        services.Configure<IdentityPasskeyOptions>(options =>
        {
            // ServerDomain defaults to the host header if not set.
            // In production, set this to your domain to prevent subdomain attacks.
            // options.ServerDomain = "arvidsonfoto.se";
            options.AuthenticatorTimeout = TimeSpan.FromMinutes(5);
        });

        // Add frontend services - all using Core now
        services.AddScoped<IGuestBookService, GuestBookService>();
        services.AddScoped<IPageCounterService, PageCounterService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IFacebookService, FacebookService>();

        // Add HttpClient for FacebookService
        services.AddHttpClient<IFacebookService, FacebookService>();

        // Add backend API services
        services.AddScoped<IApiCategoryService, ApiCategoryService>();
        services.AddScoped<IApiImageService, ApiImageService>();

        // CORS configuration for API calls from localhost and production
        var corsPolicyName = "ArvidsonFotoCorsPolicy";
        services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName, policy =>
            {
                policy.WithOrigins(
                        "https://localhost:5001",
                        "http://localhost:5000",
                        "https://www.arvidsonfoto.se",
                        "https://arvidsonfoto.se"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddControllersWithViews();
        services.AddRazorPages();

        // ===== ROUTING CONFIGURATION =====
        // Enable case-insensitive and URL-decoding routing
        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = false; // Keep original casing in generated URLs
            options.LowercaseQueryStrings = false;
        });

        // OpenAPI configuration for API documentation (using .NET 10 built-in support)
        services.AddOpenApi();

        // Configure JavaScript engine for SCSS compilation
        services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName)
                .AddV8();

        // WebOptimizer for SCSS compilation and CSS/JS minification
        services.AddWebOptimizer(pipeline =>
        {
            // Compile SCSS to CSS and minify
            pipeline.AddScssBundle("/css/site.css", "wwwroot/css/site.scss")
                .UseContentRoot();

            // Minify JavaScript files
            pipeline.AddJavaScriptBundle("/js/site.min.js", "wwwroot/js/site.js")
                .UseContentRoot();

            pipeline.AddJavaScriptBundle("/js/contactform.min.js", "wwwroot/js/ContactForm.js")
                .UseContentRoot();

            pipeline.AddJavaScriptBundle("/js/glightbox.min.js", "wwwroot/js/gLightBoxOptions.js")
                .UseContentRoot();

            pipeline.AddJavaScriptBundle("/js/passkey.min.js", "wwwroot/js/passkey.js")
                .UseContentRoot();
        });
    }

    internal static void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env, IConfiguration configuration)
    {
        // Seed in-memory database if using in-memory database
        var useInMemoryDb = Environment.GetEnvironmentVariable("CODESPACES") != null || 
                           Environment.GetEnvironmentVariable("GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN") != null ||
                           configuration.GetConnectionString("UseInMemoryDatabase") == "true";

        if (useInMemoryDb)
        {
            using (var scope = app.Services.CreateScope())
            {
                var coreContext = scope.ServiceProvider.GetRequiredService<ArvidsonFotoCoreDbContext>();
                coreContext.Database.EnsureCreated();
                
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
                
                if (allCategoryIds.Any())
                {
                    categoryService.GetCategoryNamesBulk(allCategoryIds);
                    categoryService.GetCategoryPathsBulk(allCategoryIds);
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
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
        }
        else
        {
            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        // Only use HTTPS redirection in production, not in development/Codespaces
        if (!env.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        // WebOptimizer middleware - add before UseStaticFiles
        app.UseWebOptimizer();
        
        app.UseStaticFiles();

        // Add input validation middleware to prevent SQL injection and malicious input
        app.UseMiddleware<InputValidationMiddleware>();

        app.UseRouting();

        // CORS middleware - must be after UseRouting and before UseAuthorization
        app.UseCors("ArvidsonFotoCorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        // Map Aspire default endpoints (health checks, alive checks)
        app.MapDefaultEndpoints();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        // .NET 10: endpoint-based static asset serving with fingerprinting and compression
        app.MapStaticAssets();

        // Passkey (WebAuthn) endpoints (excluded from OpenAPI/Scalar). Used by passkey.js.
        app.MapPasskeyEndpoints();
        
        // OpenAPI endpoints - only in development
        if (env.IsDevelopment())
        {
            // Native .NET 10 OpenAPI JSON endpoint
            app.MapOpenApi();
            
            // Scalar UI for interactive API documentation  
            app.MapScalarApiReference(options =>
            {
                options
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
    private static bool IsUsingInMemoryDatabase(IConfiguration configuration) =>
        Environment.GetEnvironmentVariable("CODESPACES") != null ||
        Environment.GetEnvironmentVariable("GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN") != null ||
        configuration.GetConnectionString("UseInMemoryDatabase") == "true";

    /// <summary>
    /// Returns a sanitized connection string safe for logging by replacing any password value
    /// with "***" so credentials are never written to logs or the Aspire dashboard.
    /// </summary>
    private static string SanitizeConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "(not configured)";

        return System.Text.RegularExpressions.Regex.Replace(
            connectionString,
            @"(?i)(password|pwd)\s*=\s*[^;]+",
            "$1=***");
    }
}