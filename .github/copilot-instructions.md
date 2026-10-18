# Copilot Instructions

## Project Guidelines
- For this NuGet upgrade, use exactly .NET 11 RC2 package version `11.0.0-rc.2.26504.105` for applicable framework packages; do not downgrade to stable 10.0.12. Report in Swedish.

## Code Quality
- For build and code-style warning cleanup, fix warnings without suppressing them and reduce the total below 50.

## Game Image API Guidelines
- Include an `IsGameSelectable` flag on each image.
- A disabled category must make all its images effectively unavailable to games, even when an individual image flag is true.
- Keep individual image flags manageable in admin.