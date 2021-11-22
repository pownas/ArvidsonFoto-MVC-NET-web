using Microsoft.EntityFrameworkCore;

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
        services.AddDbContext<ArvidsonFotoDbContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")));

        //Lägger till Services:
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IGuestBookService, GuestBookService>();
        services.AddScoped<IPageCounterService, PageCounterService>();

        services.AddControllersWithViews();
        services.AddRazorPages(); //Tror att Razor-Pages kan behövas... 
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
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
        app.UseHttpsRedirection();
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
        });
    }
}