# Passkey endpoints (WebAuthn) refactor + OpenAPI/Scalar hiding

## Summary
The two passkey-related Minimal API endpoints were moved out of `Program.cs` into a dedicated endpoint mapping extension, and they were excluded from OpenAPI generation so they do not show up in Scalar (`/scalar/v1`) or in the generated `openapi` document.

## Why
- Keep `Program.cs` focused on application startup configuration.
- Keep passkey/WebAuthn endpoints grouped and reusable.
- Avoid exposing internal/authentication helper endpoints in API docs.

## What changed

### 1) New endpoint mapping file
Created:
- `ArvidsonFoto/Security/PasskeyEndpointRouteBuilderExtensions.cs`

This file adds an extension method:
- `MapPasskeyEndpoints(this IEndpointRouteBuilder endpoints)`

It registers two endpoints under an `/Account` route group:
- `POST /Account/PasskeyCreationOptions`
  - Requires authentication (`RequireAuthorization()`)
  - Returns JSON created via `SignInManager.MakePasskeyCreationOptionsAsync(...)`
  - Antiforgery disabled (`DisableAntiforgery()`) because it is called by `passkey.js`

- `POST /Account/PasskeyRequestOptions`
  - Allows anonymous (`AllowAnonymous()`)
  - Returns JSON created via `SignInManager.MakePasskeyRequestOptionsAsync(...)`
  - Antiforgery disabled (`DisableAntiforgery()`)

### 2) Registration from `Program.cs`
In `ConfigureMiddleware(...)`, the inline `app.MapPost(...)` blocks were replaced with a single call:
- `app.MapPasskeyEndpoints();`

### 3) Hidden from OpenAPI / Scalar
The extension maps the endpoints inside a route group configured with:
- `ExcludeFromDescription()`

That flag prevents the endpoints from being included in the OpenAPI document produced by `app.MapOpenApi()`, so they won’t appear in Scalar UI.

## Notes / customization
- If you want the endpoints to live under the Identity area route (`/Identity/Account/...`) instead of `/Account/...`, change the route group prefix in `MapPasskeyEndpoints()`.
- If at some point you do want them visible in OpenAPI, remove `ExcludeFromDescription()` (or apply it only to selected endpoints).
