# ArvidsonFoto.Tests.E2E

End-to-end tests for ArvidsonFoto using Playwright.

## Overview

This project contains automated UI tests that verify the contact form functionality on the ArvidsonFoto website. The tests use Playwright for browser automation and capture screenshots of the UI.

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
1. Install Playwright browsers:
   ```bash
   pwsh bin/Debug/net10.0/playwright.ps1 install chromium
   ```

2. Start the ArvidsonFoto application:
   ```bash
   cd ../ArvidsonFoto
   dotnet run --urls "https://localhost:5001"
   ```

### Run Tests
```bash
dotnet test
```

### Run Tests with Verbose Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Test Configuration

- **Base URL**: `https://localhost:5001` (configurable in ContactFormTests.cs)
- **Browser**: Chromium (headless mode)
- **HTTPS**: Ignores certificate errors for local development

## Notes

- Tests run in headless mode by default. Set `Headless = false` in the test to see browser actions.
- Screenshots are automatically captured during test execution.
- The tests verify both successful form interaction and error handling.
