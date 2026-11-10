# ArvidsonFoto.AppHost - Aspire Orchestrator

This project is the Aspire AppHost for the ArvidsonFoto solution. It starts two instances of the same web project and provides the Aspire Dashboard with OpenTelemetry for local development.

## Available instances

- **arvidsonfoto**: the main website.
- **arvidsonfoto-dev-and-api-portal**: the development launch profile, with admin pages and Development-only Scalar/OpenAPI endpoints.

Use the endpoint URLs displayed by Aspire; configured launch-profile ports may change. The dashboard is normally available at `http://localhost:15888` or its HTTPS endpoint.

## Quick start

Install the SDK version pinned by the repository's `global.json` (`11.0.100`), then run from the repository root:

```powershell
dotnet run --project ArvidsonFoto.AppHost
```

Docker is not required for the default configuration. `ArvidsonFoto/appsettings.Development.json` selects an in-memory database, so data is temporary and seeded for development. The AppHost does not add a SQL Server container.

In Visual Studio, set `ArvidsonFoto.AppHost` as the startup project and start debugging. Select endpoints from the Aspire Dashboard rather than relying on hard-coded ports.

## Development endpoints

The development instance serves the following paths:

- `/dev` or `/admin`: development landing page.
- `/scalar/v1`: interactive API documentation.
- `/openapi/v1.json`: OpenAPI schema.
- `/UploadAdmin`: administration (sign-in required).

Scalar and OpenAPI are registered only in the Development environment.

## Database configuration

`ArvidsonFoto.AppHost/AppHost.cs` reads the web project's appsettings and passes `ConnectionStrings:UseInMemoryDatabase` to both instances. When it is `true`, each process uses an in-memory database and its data is lost when that process stops.

To connect to SQL Server, set `UseInMemoryDatabase` to `false` and supply a valid `ConnectionStrings:DefaultConnection` through local, non-versioned configuration. The AppHost forwards that connection string; it does not provision a server or apply database migrations. See [wiki/DatabaseConfiguration.md](../wiki/DatabaseConfiguration.md).

Never commit database credentials or other secrets.

## Observability

The Aspire Dashboard exposes resource state, console output, traces and metrics. For trace setup and known correlation behavior, see [wiki/OPENTELEMETRY_TRACING.md](../wiki/OPENTELEMETRY_TRACING.md).

## Troubleshooting

- Check the AppHost console and Dashboard for the endpoint actually assigned to each instance.
- If a port is already in use, stop the process using it or adjust the relevant launch profile.
- If Scalar is unavailable, verify that you opened the development instance and that it is running in Development.
- For a database connection failure, check `UseInMemoryDatabase`, `DefaultConnection`, server access and schema state.
