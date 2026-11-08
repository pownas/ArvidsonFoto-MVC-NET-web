using Microsoft.AspNetCore.Identity;

namespace ArvidsonFoto.Areas.Identity.Data;

public static class ArvidsonFotoIdentityDbSeeder
{
    public static async Task SeedTestAccountAsync(
        IServiceProvider services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ArvidsonFotoIdentityDbSeeder));

        if (!environment.IsDevelopment())
        {
            logger.LogInformation("Skipping Identity test account seeding outside Development.");
            return;
        }

        if (!configuration.GetValue<bool>("DatabaseSettings:SeedTestAccount"))
        {
            logger.LogInformation("Identity test account seeding is disabled by configuration.");
            return;
        }

        var email = configuration["DatabaseSettings:TestAccount:Email"];
        var password = configuration["DatabaseSettings:TestAccount:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "DatabaseSettings:TestAccount:Email and Password must be configured when test account seeding is enabled.");
        }

        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ArvidsonFotoUser>>();
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ArvidsonFotoUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(error => $"{error.Code}: {error.Description}"));
                throw new InvalidOperationException($"Failed to create the local Identity test account: {errors}");
            }

            logger.LogInformation("Created local Identity test account {Email}.", email);
            return;
        }

        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(error => $"{error.Code}: {error.Description}"));
                throw new InvalidOperationException($"Failed to confirm the local Identity test account: {errors}");
            }
        }

        logger.LogInformation("Local Identity test account {Email} already exists.", email);
    }
}