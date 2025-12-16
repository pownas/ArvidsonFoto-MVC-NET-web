# ArvidsonFoto.AppHost - Aspire Orchestrator

This project is the Aspire AppHost for the ArvidsonFoto solution. It orchestrates all application components and provides access to the Aspire Dashboard with OpenTelemetry for local and Codespaces development.

## What is Aspire AppHost?

The AppHost is the entry point for running the entire ArvidsonFoto solution with .NET Aspire orchestration. It provides:

- **Aspire Dashboard** - Visual monitoring dashboard at http://localhost:15888
- **OpenTelemetry** - Distributed tracing, metrics, and logs
- **SQL Server** - Containerized SQL Server for development
- **Service Discovery** - Automatic service-to-service communication
- **Health Checks** - Monitor application health

## Quick Start

### Prerequisites

1. **Docker Desktop** must be running
   - Download from https://www.docker.com/products/docker-desktop/
   - Make sure Docker is started before running AppHost

2. **.NET 10 SDK** (already installed in this project)

### Running the AppHost

From the repository root:

```bash
# Start the AppHost
dotnet run --project ArvidsonFoto.AppHost

# Or from the AppHost directory
cd ArvidsonFoto.AppHost
dotnet run
```

This will:
1. Start SQL Server in a Docker container
2. Start the ArvidsonFoto web application
3. Open the Aspire Dashboard in your browser

### Aspire Dashboard

Once running, the Aspire Dashboard will be available at:
- **Dashboard URL**: http://localhost:15888 (or https://localhost:19234)

The dashboard provides:
- **Resources** - Status of all services and containers
- **Console Logs** - Real-time logs from all components
- **Traces** - Distributed tracing for request flows
- **Metrics** - Performance metrics and graphs
- **Structured Logs** - Advanced log filtering and viewing

## Using in Visual Studio

1. Set `ArvidsonFoto.AppHost` as the startup project
2. Press **F5** or click **Start Debugging**
3. The Aspire Dashboard will open automatically

## Using in Visual Studio Code

1. Open the repository folder
2. Use the Run and Debug panel (Ctrl+Shift+D)
3. Select "Aspire: AppHost" configuration
4. Press F5 to start

## Using in GitHub Codespaces

The AppHost works seamlessly in Codespaces:

1. Open the repository in a Codespace
2. Run `dotnet run --project ArvidsonFoto.AppHost`
3. Codespaces will automatically forward the ports
4. Click on the forwarded port to open the dashboard

## Project Structure

### Components Orchestrated

#### SQL Server
- **Container Image**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Database**: ArvidsonFotoDb
- **Connection**: Automatically configured for the web app
- **Data Persistence**: Container data persists across runs

#### ArvidsonFoto Web Application
- **Project**: ArvidsonFoto/ArvidsonFoto.csproj
- **Dependencies**: SQL Server database
- **OpenTelemetry**: Enabled via ServiceDefaults
- **Health Checks**: Available at /health and /alive endpoints

## Configuration

### AppHost.cs

The main configuration file that defines all resources:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// SQL Server with persistent data
var sqlServer = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("ArvidsonFotoDb");

// Main web application with SQL reference
var arvidsonFoto = builder.AddProject<Projects.ArvidsonFoto>("arvidsonfoto")
    .WithReference(sqlServer)
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

### Adding More Services

To add more services (like Redis, RabbitMQ, etc.):

```csharp
// Add Redis cache
var redis = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent);

var arvidsonFoto = builder.AddProject<Projects.ArvidsonFoto>("arvidsonfoto")
    .WithReference(sqlServer)
    .WithReference(redis)
    .WithExternalHttpEndpoints();
```

### Environment Variables

Configure behavior via environment variables in appsettings.json:

```json
{
  "OTEL_EXPORTER_OTLP_ENDPOINT": "http://localhost:4317",
  "ASPNETCORE_ENVIRONMENT": "Development"
}
```

## OpenTelemetry Integration

### Traces

View distributed traces in the dashboard:
1. Navigate to **Traces** tab
2. Filter by service, operation, or time range
3. Click on a trace to see the full request flow
4. Analyze performance bottlenecks

### Metrics

Available metrics:
- **HTTP metrics**: Request count, duration, status codes
- **Runtime metrics**: CPU usage, memory, garbage collection
- **.NET metrics**: Thread pool, exceptions, etc.

### Logs

Structured logging with:
- **Filtering** by level, service, or search terms
- **Context** - See related traces for each log entry
- **Scopes** - Hierarchical log organization

## Troubleshooting

### Docker Not Running

```
Error: Docker is not running
```

**Solution**: Start Docker Desktop

### Port Already in Use

```
Error: Address already in use
```

**Solution**: 
- Stop other applications using ports 15888 or 19234
- Or change ports in launchSettings.json

### SQL Server Fails to Start

```
Error: SQL Server container failed to start
```

**Solutions**:
1. Check Docker has enough memory (at least 2GB)
2. View container logs in Aspire Dashboard
3. Try removing the container: `docker rm -f aspire-sql-1`

### Connection Strings

The AppHost automatically configures connection strings. If you need to override:

```json
{
  "ConnectionStrings": {
    "ArvidsonFotoDb": "Server=localhost,1433;Database=ArvidsonFotoDb;..."
  }
}
```

## Health Checks

The application exposes health check endpoints:

- **/health** - Overall application health
- **/alive** - Liveness probe (always returns healthy if running)

Access via:
- Dashboard: Resources tab → Click on service → View health status
- Direct: http://localhost:PORT/health

## Service Defaults

The `ArvidsonFoto.ServiceDefaults` project provides:
- OpenTelemetry configuration
- Health check defaults
- Service discovery
- Resilience patterns (retry, circuit breaker)

Every service in the solution should reference ServiceDefaults:

```xml
<ProjectReference Include="../ArvidsonFoto.ServiceDefaults/ArvidsonFoto.ServiceDefaults.csproj" />
```

And add in Program.cs:

```csharp
builder.AddServiceDefaults();
app.MapDefaultEndpoints();
```

## Benefits of Using Aspire

### For Local Development

1. **One Command Start** - Start entire solution with dependencies
2. **No Manual Setup** - SQL Server, Redis, etc. in containers
3. **Visual Dashboard** - See all services and logs in one place
4. **Fast Debugging** - Distributed tracing shows exact flow

### For Codespaces

1. **Instant Setup** - No database installation needed
2. **Port Forwarding** - Automatic port exposure
3. **Consistent Environment** - Same setup for all developers

### For Production Readiness

1. **Observability** - Production-ready OpenTelemetry
2. **Health Checks** - Kubernetes-ready health endpoints
3. **Service Discovery** - Microservices communication patterns
4. **Resilience** - Built-in retry and circuit breaker

## Learn More

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Aspire Dashboard](https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard)
- [OpenTelemetry](https://opentelemetry.io/)
- [Project Documentation](../docs/ASPIRE.md)

## Next Steps

1. **Start the AppHost** - Run it and explore the dashboard
2. **Make requests** - Use the web app and see traces in the dashboard
3. **Add services** - Try adding Redis or other resources
4. **Deploy** - Use the same observability in production

---

**Note**: This is Aspire v13, the latest version with improved dashboard and OpenTelemetry integration.
