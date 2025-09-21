using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ArvidsonFoto.Core.Services;

namespace ArvidsonFoto.Core.Extensions;

/// <summary>
/// Extension methods for configuring database initialization services.
/// </summary>
public static class DatabaseInitializationExtensions
{
    /// <summary>
    /// Adds the database initialization service to the dependency injection container.
    /// This service will automatically initialize the database schema and seed data 
    /// on first startup in development environment only.
    /// </summary>
    /// <param name="services">The service collection to add the service to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDatabaseInitialization(this IServiceCollection services)
    {
        services.AddScoped<DatabaseInitializationService>();
        return services;
    }

    /// <summary>
    /// Initializes the database using the DatabaseInitializationService.
    /// This method should be called after the application is built but before it runs.
    /// 
    /// Usage example:
    /// <code>
    /// var app = builder.Build();
    /// await app.InitializeDatabaseAsync();
    /// app.Run();
    /// </code>
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task InitializeDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var initializationService = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
        await initializationService.InitializeDatabaseAsync();
    }
}