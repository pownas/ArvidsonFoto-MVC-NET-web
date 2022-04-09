using ArvidsonFoto.Areas.Identity.Data;
using ArvidsonFoto.Data;
using Microsoft.EntityFrameworkCore;

[assembly: HostingStartup(typeof(ArvidsonFoto.Areas.Identity.IdentityHostingStartup))]
namespace ArvidsonFoto.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ArvidsonFotoIdentityContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DefaultConnection")));

                services.AddDefaultIdentity<ArvidsonFotoUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ArvidsonFotoIdentityContext>();
            });
        }
    }
}