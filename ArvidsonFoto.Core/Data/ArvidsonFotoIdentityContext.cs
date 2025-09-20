using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArvidsonFoto.Core.Data;

/// <summary>
/// Databaskontext för ASP.NET Core Identity.
/// </summary>
/// <remarks>
/// Denna klass hanterar autentisering och auktorisering för applikationen
/// genom att utöka IdentityDbContext med anpassade användarmodeller.
/// </remarks>
public class ArvidsonFotoIdentityContext : IdentityDbContext<ArvidsonFotoIdentityUser>
{
    /// <summary>
    /// Initierar en ny instans av ArvidsonFotoIdentityContext.
    /// </summary>
    /// <param name="options">Databaskontext-alternativ för Identity</param>
    public ArvidsonFotoIdentityContext(DbContextOptions<ArvidsonFotoIdentityContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Konfigurerar Identity-modellen och anpassar standardbeteendet.
    /// </summary>
    /// <param name="builder">Modellbyggare för Entity Framework</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
