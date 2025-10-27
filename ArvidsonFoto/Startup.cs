using Microsoft.EntityFrameworkCore;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using ArvidsonFoto.Data;
using ArvidsonFoto.Services;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace ArvidsonFoto;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        //Lägger till Databaskoppling för appen (Identity kopplas i: /Areas/Identity/IdentityHostingStartup.cs): 
        var useInMemoryDb = Environment.GetEnvironmentVariable("CODESPACES") != null || 
                           Environment.GetEnvironmentVariable("GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN") != null ||
                           Configuration.GetConnectionString("UseInMemoryDatabase") == "true";

        if (useInMemoryDb)
        {
            // Använd In-Memory databas i Codespaces eller när konfigurerat
            services.AddDbContext<ArvidsonFotoDbContext>(options =>
                options.UseInMemoryDatabase("ArvidsonFotoInMemory"));
            services.AddDbContext<ArvidsonFotoCoreDbContext>(options =>
                options.UseInMemoryDatabase("ArvidsonFotoInMemory"));
        }
        else
        {
            // Använd SQL Server lokalt
            services.AddDbContext<ArvidsonFotoDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ArvidsonFotoCoreDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
        }

        // Lägger till Services för frontend:
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IGuestBookService, GuestBookService>();
        services.AddScoped<IPageCounterService, PageCounterService>();

        // Lägger till Services för backend API: 
        services.AddScoped<IApiCategoryService, ApiCategoryService>();
        services.AddScoped<IApiImageService, ApiImageService>();

        // Konfigurera lokalisering
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.AddControllersWithViews()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();
        services.AddRazorPages(); //Tror att Razor-Pages kan behövas... 

        // OpenAPI konfiguration för API-dokumentation (använder .NET 10 inbyggt stöd)
        services.AddOpenApi();

        // Konfigurera JavaScript engine för SCSS kompilering
        services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName)
                .AddV8();

        // WebOptimizer för SCSS kompilering och CSS/JS minifiering
        services.AddWebOptimizer(pipeline =>
        {
            // Kompilera SCSS till CSS och minifiera
            pipeline.AddScssBundle("/css/site.css", "wwwroot/css/site.scss")
                .UseContentRoot();

            // Minifiera JavaScript filer
            pipeline.AddJavaScriptBundle("/js/site.min.js", "wwwroot/js/site.js")
                .UseContentRoot();

            pipeline.AddJavaScriptBundle("/js/contactform.min.js", "wwwroot/js/ContactForm.js")
                .UseContentRoot();

            pipeline.AddJavaScriptBundle("/js/glightbox.min.js", "wwwroot/js/gLightBoxOptions.js")
                .UseContentRoot();
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Konfigurera lokalisering
        var supportedCultures = new[]
        {
            new CultureInfo("sv-SE"),
            new CultureInfo("en-US")
        };

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("sv-SE"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        // Seed in-memory database if using in-memory database
        var useInMemoryDb = Environment.GetEnvironmentVariable("CODESPACES") != null || 
                           Environment.GetEnvironmentVariable("GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN") != null ||
                           Configuration.GetConnectionString("UseInMemoryDatabase") == "true";

        if (useInMemoryDb)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ArvidsonFotoDbContext>();
                context.Database.EnsureCreated();
                context.SeedInMemoryDatabase();
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

        // WebOptimizer middleware - lägg till före UseStaticFiles
        app.UseWebOptimizer();
        
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
            
            // Native .NET 10 OpenAPI endpoint - endast i development
            if (env.IsDevelopment())
            {
                endpoints.MapOpenApi("/api/openapi.json");
            }
        });
    }
}