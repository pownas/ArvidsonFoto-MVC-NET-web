# ArvidsonFoto.Tests.E2E

End-to-end tests for ArvidsonFoto using Playwright.

## Overview

This project contains automated UI tests that verify the contact form functionality on the ArvidsonFoto website. The tests use Playwright for browser automation and capture screenshots of the UI.

The tests **start the web application automatically** via `WebApplicationFactory` and a real Kestrel server on a randomly-assigned port, so no manual `dotnet run` step is needed before running them.

## Test Coverage

### Contact Form Tests
- **ContactForm_DisplaysCorrectly**: Verifies all form fields are visible on the contact page
- **ContactForm_ShowsValidationErrors_WhenFieldsEmpty**: Tests HTML5 validation when submitting empty form
- **ContactForm_FillsOutFormCorrectly**: Tests form filling with valid data
- **ContactForm_ShowsErrorMessage_OnEmailFailure**: Tests error message display when email sending fails
- **ContactForm_ShowsSuccessMessage_AndClearsForm**: Tests success message and form clearing after successful submission

### Image Purchase Form Tests
- **ImagePurchaseForm_DisplaysCorrectly**: Verifies the image purchase page displays correctly
- **ImagePurchaseForm_FillsOutFormCorrectly**: Tests filling out the image purchase form

## Screenshots

All tests capture screenshots that are saved to the `screenshots/` directory:

- `contact-form-page.png` - Initial contact form page
- `contact-form-validation-errors.png` - Validation errors when fields are empty
- `contact-form-filled.png` - Form filled with test data
- `contact-form-success-message.png` - Success message with cleared form after email sent successfully
- `contact-form-error-message.png` - Error message with fallback email (torbjorn@arvidsonfoto.se)
- `image-purchase-form-page.png` - Image purchase form page
- `image-purchase-form-filled.png` - Image purchase form filled with data

## Running the Tests

### Prerequisites

Install Playwright browsers (only needed once after cloning):
```bash
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

Or, build the project first and then install:
```bash
dotnet build
pwsh ArvidsonFoto.Tests.E2E/bin/Debug/net10.0/playwright.ps1 install chromium
```

### Run Tests

The web application is started automatically — just run:
```bash
dotnet test
```

### Run Tests with Verbose Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Test Configuration

- **Base URL**: assigned automatically by Kestrel at runtime (random available port on `localhost`)
- **Database**: in-memory (no SQL Server needed)
- **Browser**: Chromium (headless mode)
- **HTTPS**: not used — the test server runs on plain HTTP

## How It Works

`PlaywrightWebApplicationFactory` implements `IAsyncLifetime` directly — it **does not** inherit from
`WebApplicationFactory<Program>`. This is intentional: `WebApplicationFactory` unconditionally casts
`IServer` to `TestServer` after host creation, which throws an `InvalidCastException` when Kestrel is
used instead.

Instead, `InitializeAsync` builds a real `WebApplication` using `WebApplication.CreateBuilder`, injects
test overrides (in-memory database, stub SMTP), calls `Program.ConfigureServices()` and
`Program.ConfigureMiddleware()` to replicate the production pipeline exactly, and binds Kestrel to a
pre-allocated free port via `builder.WebHost.UseUrls(...)`.

`ContactFormTests` consumes the factory via `IClassFixture<PlaywrightWebApplicationFactory>` — xUnit
starts the Kestrel server once before the first test in the class, and stops it after the last test.
Each individual test gets its own Playwright browser instance via `IAsyncLifetime`.


## Notes

- Tests run in headless mode by default. Set `Headless = false` in `ContactFormTests` to watch the browser.
- Screenshots are automatically captured during test execution.
- The tests verify both successful form interaction and error handling.
