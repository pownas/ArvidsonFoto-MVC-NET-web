using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ArvidsonFoto.Core.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ArvidsonFoto.Core.Services;

/// <summary>
/// Service responsible for initializing the database with schema and seed data in development environment only.
/// This service ensures that:
/// - Database schema is created or updated via migrations
/// - Essential seed data is populated on first startup
/// - Operations are idempotent (safe to run multiple times)
/// - Only runs in development environment for security
/// </summary>
public class DatabaseInitializationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<DatabaseInitializationService> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the DatabaseInitializationService.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies</param>
    /// <param name="environment">The hosting environment to check if we're in development</param>
    /// <param name="logger">Logger for tracking initialization progress and errors</param>
    /// <param name="configuration">Configuration to check for database seeding settings</param>
    public DatabaseInitializationService(
        IServiceProvider serviceProvider,
        IHostEnvironment environment,
        ILogger<DatabaseInitializationService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _environment = environment;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Initializes the database with schema and seed data if running in development environment.
    /// This method is idempotent and safe to call on every application startup.
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InitializeDatabaseAsync()
    {
        await InitializeDatabaseAsync(_serviceProvider, _environment, _logger, _configuration);
    }

    /// <summary>
    /// Initializes the database with schema and seed data if running in development environment.
    /// This method is idempotent and safe to call on every application startup.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies</param>
    /// <param name="environment">The hosting environment to check if we're in development</param>
    /// <param name="logger">Logger for tracking initialization progress and errors</param>
    /// <param name="configuration">Configuration to check for database seeding settings</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static async Task InitializeDatabaseAsync(
        IServiceProvider serviceProvider,
        IHostEnvironment environment,
        ILogger logger,
        IConfiguration configuration)
    {
        // Only initialize database in development environment for security
        if (!environment.IsDevelopment())
        {
            logger.LogInformation("Skipping database initialization - not in development environment");
            return;
        }

        // Check if database initialization is enabled via configuration
        var enableSeeding = configuration.GetValue<bool>("DatabaseSettings:EnableSeeding", false);
        if (!enableSeeding)
        {
            logger.LogInformation("Database initialization disabled via configuration setting 'DatabaseSettings:EnableSeeding'");
            return;
        }

        logger.LogInformation("Starting database initialization for development environment");

        try
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            // Initialize main application database
            await InitializeMainDatabaseAsync(services, logger);

            // Initialize Identity database if available
            await InitializeIdentityDatabaseAsync(services, logger);

            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization");
            throw; // Re-throw to prevent application startup with incomplete database
        }
    }

    /// <summary>
    /// Initializes the main application database (ArvidsonFotoDbContext) with schema and seed data.
    /// </summary>
    private static async Task InitializeMainDatabaseAsync(IServiceProvider services, ILogger logger)
    {
        logger.LogInformation("🏁 Starting main database initialization...");
        try
        {
            logger.LogInformation("🔧 Getting ArvidsonFotoDbContext from services...");
            var context = services.GetRequiredService<ArvidsonFotoDbContext>();

            logger.LogInformation("🔍 Checking database provider type...");
            // Only migrate if using a relational database provider (not in-memory)
            if (context.Database.IsRelational())
            {
                logger.LogInformation("📊 Using relational database - checking for pending migrations");
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Applying {Count} pending migrations to main database", pendingMigrations.Count());
                    await context.Database.MigrateAsync();
                }
                else
                {
                    logger.LogInformation("Main database is up to date - no migrations needed");
                }
            }
            else
            {
                logger.LogInformation("💾 Using in-memory database - ensuring database is created");
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("✅ In-memory database created successfully");
            }

            logger.LogInformation("Checking if seed data is needed for main database");
            await SeedMainDatabaseAsync(context, logger, services);

            logger.LogInformation("Main database initialization completed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize main database: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Initializes the Identity database if the service is available.
    /// Supports both ArvidsonFotoIdentityContext and any other IdentityDbContext.
    /// </summary>
    private static async Task InitializeIdentityDatabaseAsync(IServiceProvider services, ILogger logger)
    {
        try
        {
            // Try ArvidsonFotoIdentityContext first (used in API)
            var identityContext = services.GetService<ArvidsonFotoIdentityContext>();
            if (identityContext != null)
            {
                if (identityContext.Database.IsRelational())
                {
                    logger.LogInformation("Checking ArvidsonFotoIdentityContext for pending migrations");
                    var pendingMigrations = await identityContext.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        logger.LogInformation("Applying {Count} pending migrations to ArvidsonFotoIdentityContext", pendingMigrations.Count());
                        await identityContext.Database.MigrateAsync();
                    }
                    else
                    {
                        logger.LogInformation("ArvidsonFotoIdentityContext is up to date - no migrations needed");
                    }
                }
                else
                {
                    logger.LogInformation("Using in-memory database - ensuring ArvidsonFotoIdentityContext is created");
                    await identityContext.Database.EnsureCreatedAsync();
                }
                logger.LogInformation("ArvidsonFotoIdentityContext initialization completed");
                return;
            }

            // Try to find any IdentityDbContext (used in Web applications)
            var identityDbContexts = services.GetServices<DbContext>()
                .Where(ctx => ctx.GetType().IsSubclassOf(typeof(IdentityDbContext)) ||
                             (ctx.GetType().BaseType?.IsGenericType == true &&
                              ctx.GetType().BaseType?.GetGenericTypeDefinition() == typeof(IdentityDbContext<>)))
                .ToList();

            if (identityDbContexts.Any())
            {
                foreach (var context in identityDbContexts)
                {
                    if (context.Database.IsRelational())
                    {
                        logger.LogInformation("Checking {ContextType} for pending migrations", context.GetType().Name);
                        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                        if (pendingMigrations.Any())
                        {
                            logger.LogInformation("Applying {Count} pending migrations to {ContextType}", pendingMigrations.Count(), context.GetType().Name);
                            await context.Database.MigrateAsync();
                        }
                        else
                        {
                            logger.LogInformation("{ContextType} is up to date - no migrations needed", context.GetType().Name);
                        }
                    }
                    else
                    {
                        logger.LogInformation("Using in-memory database - ensuring {ContextType} is created", context.GetType().Name);
                        await context.Database.EnsureCreatedAsync();
                    }
                    logger.LogInformation("{ContextType} initialization completed", context.GetType().Name);
                }
                return;
            }

            logger.LogInformation("No Identity database context found - skipping Identity database initialization");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize Identity database: {ErrorMessage}", ex.Message);

            // Don't throw for Identity database issues if it's a table already exists error
            if (ex.Message.Contains("There is already an object named") || ex.Message.Contains("AspNetRoles"))
            {
                logger.LogWarning("Identity tables already exist - continuing with application startup");
                return;
            }

            throw;
        }
    }

    /// <summary>
    /// Seeds the main database with initial data if it's empty.
    /// This method is idempotent - it checks for existing data before seeding.
    /// </summary>
    private static async Task SeedMainDatabaseAsync(ArvidsonFotoDbContext context, ILogger logger, IServiceProvider services)
    {
        logger.LogInformation("🔍 Checking if database already contains data...");

        // Check if database already has data (idempotent check)
        if (await context.TblMenus.AnyAsync())
        {
            logger.LogInformation("Database already contains data - skipping seed operation");
            return;
        }

        logger.LogInformation("Database is empty - applying seed data");

        try
        {
            // Apply seed data manually using the existing seeder logic
            // This approach is more reliable than using a separate context with EnsureCreated
            await SeedDataManuallyAsync(context, logger, services);

            logger.LogInformation("Seed data applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply seed data");
            throw;
        }
    }

    /// <summary>
    /// Manually applies seed data to the database context.
    /// This uses the ArvidsonFotoDbSeeder data but can limit quantities for faster testing.
    /// When limiting images, ensures that all categories referenced by those images are also included.
    /// </summary>
    private static async Task SeedDataManuallyAsync(ArvidsonFotoDbContext context, ILogger logger, IServiceProvider services)
    {
        // Get configuration from DI container
        var configuration = services.GetService<IConfiguration>();

        // Get configuration to check for limited seeding - try config first, then environment variables
        var limitSeedData = configuration?.GetValue<bool>("DatabaseSettings:LimitSeedData", false) ??
                           Environment.GetEnvironmentVariable("DatabaseSettings__LimitSeedData") == "true";
        var maxImages = configuration?.GetValue<int>("DatabaseSettings:MaxImages", 50) ??
                       int.Parse(Environment.GetEnvironmentVariable("DatabaseSettings__MaxImages") ?? "50");
        var maxMenus = configuration?.GetValue<int>("DatabaseSettings:MaxMenus", 100) ??
                      int.Parse(Environment.GetEnvironmentVariable("DatabaseSettings__MaxMenus") ?? "100");

        if (limitSeedData)
        {
            logger.LogInformation("🚀 Using limited seed data for faster tests - Max Images: {MaxImages}, Max Menus: {MaxMenus}", maxImages, maxMenus);
        }
        else
        {
            logger.LogInformation("📊 Using full seed data from ArvidsonFotoDbSeeder");
        }

        // Add Guestbook entries (always add all - they're lightweight)
        logger.LogInformation("Adding seed data for TblGb ({Count} entries)", ArvidsonFoto.Core.Data.ArvidsonFotoDbSeeder.GuestbookEntries.Count);
        context.TblGbs.AddRange(ArvidsonFoto.Core.Data.ArvidsonFotoDbSeeder.GuestbookEntries);

        // Smart seeding: When limiting data, ensure categories exist for all images
        var allMenuCategories = ArvidsonFoto.Core.Data.ArvidsonFotoDbSeeder.MenuCategories;
        var allImages = ArvidsonFoto.Core.Data.ArvidsonFotoDbSeeder.Images;

        List<TblMenu> menuCategoriesToSeed;
        List<TblImage> imagesToSeed;

        if (limitSeedData)
        {
            // Step 1: Get the limited set of images
            imagesToSeed = allImages.Take(maxImages).ToList();
            logger.LogInformation("🔄 Selected {Count} images for limited seeding", imagesToSeed.Count);

            // Step 2: Get all category IDs that these images reference
            var requiredCategoryIds = imagesToSeed
                .Where(img => img.ImageCategoryId.HasValue)
                .Select(img => img.ImageCategoryId!.Value)
                .Distinct()
                .ToHashSet();

            // Step 3: Get all family IDs that these images reference
            var requiredFamilyIds = imagesToSeed
                .Where(img => img.ImageFamilyId.HasValue)
                .Select(img => img.ImageFamilyId!.Value)
                .Distinct()
                .ToHashSet();

            // Step 4: Get all main family IDs that these images reference
            var requiredMainFamilyIds = imagesToSeed
                .Where(img => img.ImageMainFamilyId.HasValue)
                .Select(img => img.ImageMainFamilyId!.Value)
                .Distinct()
                .ToHashSet();

            // Combine all required category IDs
            var allRequiredCategoryIds = requiredCategoryIds
                .Union(requiredFamilyIds)
                .Union(requiredMainFamilyIds)
                .ToHashSet();

            logger.LogInformation("📋 Images require {Count} specific categories: [{Categories}]",
                allRequiredCategoryIds.Count,
                string.Join(", ", allRequiredCategoryIds.OrderBy(x => x).Take(10)) + (allRequiredCategoryIds.Count > 10 ? "..." : ""));

            // Step 5: Find all menu categories that are required by the images
            var requiredMenuCategories = allMenuCategories
                .Where(menu => menu.MenuCategoryId.HasValue && allRequiredCategoryIds.Contains(menu.MenuCategoryId.Value))
                .ToList();

            // Step 6: Add parent categories (recursive) to ensure complete hierarchy
            var menuCategoriesWithParents = new HashSet<TblMenu>(requiredMenuCategories);

            foreach (var menu in requiredMenuCategories.ToList())
            {
                AddParentCategoriesRecursive(menu, allMenuCategories, menuCategoriesWithParents);
            }

            // Step 7: If we still have space under maxMenus, add more popular categories
            var currentCount = menuCategoriesWithParents.Count;
            if (currentCount < maxMenus)
            {
                var additionalNeeded = maxMenus - currentCount;
                var existingIds = menuCategoriesWithParents.Select(m => m.MenuCategoryId).Where(id => id.HasValue).Select(id => id!.Value).ToHashSet();

                var additionalCategories = allMenuCategories
                    .Where(m => m.MenuCategoryId.HasValue && !existingIds.Contains(m.MenuCategoryId.Value))
                    .OrderBy(m => m.MenuParentCategoryId == 0 ? 0 : 1) // Prefer main categories
                    .ThenBy(m => m.Id) // Then by original order
                    .Take(additionalNeeded)
                    .ToList();

                foreach (var additional in additionalCategories)
                {
                    menuCategoriesWithParents.Add(additional);
                }

                logger.LogInformation("➕ Added {Count} additional categories to reach target of {MaxMenus}",
                    additionalCategories.Count, maxMenus);
            }

            menuCategoriesToSeed = menuCategoriesWithParents.OrderBy(m => m.Id).ToList();

            logger.LogInformation("✅ Final menu categories for seeding: {Count} (required: {RequiredCount}, with parents: {WithParentsCount})",
                menuCategoriesToSeed.Count, requiredMenuCategories.Count, currentCount);
        }
        else
        {
            // Use all data when not limiting
            menuCategoriesToSeed = allMenuCategories;
            imagesToSeed = allImages;
        }

        logger.LogInformation("Adding seed data for TblMenu ({Count} entries)", menuCategoriesToSeed.Count);
        context.TblMenus.AddRange(menuCategoriesToSeed);

        logger.LogInformation("Adding seed data for TblImage ({Count} entries)", imagesToSeed.Count);
        context.TblImages.AddRange(imagesToSeed);

        // Add Page Counters (always add all - they're lightweight)
        logger.LogInformation("Adding seed data for TblPageCounter ({Count} entries)", ArvidsonFoto.Core.Data.ArvidsonFotoDbSeeder.PageCounters.Count);
        context.TblPageCounter.AddRange(ArvidsonFoto.Core.Data.ArvidsonFotoDbSeeder.PageCounters);

        // Save all changes
        logger.LogInformation("💾 Saving seed data to database...");
        await context.SaveChangesAsync();

        if (limitSeedData)
        {
            logger.LogInformation("✅ Limited seed data applied successfully for fast testing!");
            logger.LogInformation("🔗 All {ImageCount} images have their required categories available!", imagesToSeed.Count);
        }
        else
        {
            logger.LogInformation("✅ Full seed data applied successfully!");
        }
    }

    /// <summary>
    /// Recursively adds parent categories to ensure complete category hierarchy
    /// </summary>
    private static void AddParentCategoriesRecursive(TblMenu menu, List<TblMenu> allMenuCategories, HashSet<TblMenu> result)
    {
        if (menu.MenuParentCategoryId.HasValue && menu.MenuParentCategoryId.Value > 0)
        {
            var parentMenu = allMenuCategories.FirstOrDefault(m => m.MenuCategoryId.HasValue && m.MenuCategoryId.Value == menu.MenuParentCategoryId.Value);
            if (parentMenu != null && result.Add(parentMenu)) // Add returns true if item was not already in set
            {
                // Recursively add grandparents, etc.
                AddParentCategoriesRecursive(parentMenu, allMenuCategories, result);
            }
        }
    }
}