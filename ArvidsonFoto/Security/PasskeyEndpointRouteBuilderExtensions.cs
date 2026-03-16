using Microsoft.AspNetCore.Identity;
using ArvidsonFoto.Areas.Identity.Data;

namespace ArvidsonFoto.Security;

public static class PasskeyEndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapPasskeyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/Account")
            .ExcludeFromDescription();

        group.MapPost("/PasskeyCreationOptions", async (
                HttpContext context,
                UserManager<ArvidsonFotoUser> userManager,
                SignInManager<ArvidsonFotoUser> signInManager) =>
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user is null)
                {
                    return Results.NotFound();
                }

                var userId = await userManager.GetUserIdAsync(user);
                var userName = await userManager.GetUserNameAsync(user) ?? string.Empty;

                var optionsJson = await signInManager.MakePasskeyCreationOptionsAsync(new PasskeyUserEntity
                {
                    Id = userId,
                    Name = userName,
                    DisplayName = userName,
                });

                return TypedResults.Content(optionsJson, "application/json");
            })
            .RequireAuthorization()
            .DisableAntiforgery();

        group.MapPost("/PasskeyRequestOptions", async (
                SignInManager<ArvidsonFotoUser> signInManager,
                UserManager<ArvidsonFotoUser> userManager,
                string? username) =>
            {
                var user = string.IsNullOrEmpty(username)
                    ? null
                    : await userManager.FindByNameAsync(username);

                var optionsJson = await signInManager.MakePasskeyRequestOptionsAsync(user);

                return TypedResults.Content(optionsJson, "application/json");
            })
            .AllowAnonymous()
            .DisableAntiforgery();

        return group;
    }
}
