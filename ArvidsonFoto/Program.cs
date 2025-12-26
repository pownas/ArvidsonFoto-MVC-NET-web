using Microsoft.EntityFrameworkCore;
using JavaScriptEngineSwitcher.V8;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using ArvidsonFoto.Data;
using ArvidsonFoto.Services;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Security;
using ArvidsonFoto.Areas.Identity.Data;
using IdentityContext = ArvidsonFoto.Areas.Identity.Data.ArvidsonFotoIdentityContext;

namespace ArvidsonFoto;

public class Program
{
    public static void Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            //.WriteTo.Console() //Kräver nuget paketet: Serilog.Sinks.Console
            .WriteTo.File("logs\\appLog.txt", rollingInterval: RollingInterval.Day) //Bör kanske försöka byta till Serilog DB-loggning...
            .CreateLogger();

        try
        {
            Log.Information("Starting web application");
            
            var builder = WebApplication.CreateBuilder(args);

            // Add Aspire service defaults (observability, health checks, resilience, service discovery)
            builder.AddServiceDefaults();

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

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        // Database configuration
        var useInMemoryDb = Environment.GetEnvironmentVariable("CODESPACES") != null || 
                           Environment.GetEnvironmentVariable("GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN") != null ||
                           configuration.GetConnectionString("UseInMemoryDatabase") == "true";

        if (useInMemoryDb)
        {
            // Use In-Memory database in Codespaces or when configured
            services.AddDbContext<ArvidsonFotoDbContext>(options =>
                options.UseInMemoryDatabase("ArvidsonFotoInMemory"));
            services.AddDbContext<ArvidsonFotoCoreDbContext>(options =>
                options.UseInMemoryDatabase("ArvidsonFotoInMemory"));
            services.AddDbContext<IdentityContext>(options =>
                options.UseInMemoryDatabase("ArvidsonFotoInMemory"));
        }
        else
        {
            // Use SQL Server locally
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ArvidsonFotoDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDbContext<ArvidsonFotoCoreDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(connectionString));
        }

        // Identity configuration (moved from IdentityHostingStartup.cs)
        services.AddDefaultIdentity<ArvidsonFotoUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<IdentityContext>();

        // Add frontend services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IGuestBookService, GuestBookService>();
        services.AddScoped<IPageCounterService, PageCounterService>();
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

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
        }
        else
        {
            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
            //app.UseExceptionHandler("/Home/Error");
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
        
        // Native .NET 10 OpenAPI endpoint - only in development
        if (env.IsDevelopment())
        {
            app.MapOpenApi("/api/openapi.json");
        }
    }
}