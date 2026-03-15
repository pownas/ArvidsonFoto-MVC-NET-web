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

`PlaywrightWebApplicationFactory` inherits from `WebApplicationFactory<Program>` (the recommended
approach per the [ASP.NET Core integration-test docs](https://learn.microsoft.com/aspnet/core/test/integration-tests)).
It overrides `ConfigureWebHost` to inject an in-memory database and dummy SMTP settings, and overrides
`CreateHost` to swap the default in-memory TestServer for a real Kestrel listener on a random port.
`ContactFormTests.InitializeAsync` calls `EnsureStarted()` to trigger host creation and reads the
resulting URL before handing it to Playwright.

## Notes

- Tests run in headless mode by default. Set `Headless = false` in `ContactFormTests` to watch the browser.
- Screenshots are automatically captured during test execution.
- The tests verify both successful form interaction and error handling.
