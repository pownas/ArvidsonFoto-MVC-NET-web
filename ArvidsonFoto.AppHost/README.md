# ArvidsonFoto.AppHost - Aspire Orchestrator

This project is the Aspire AppHost for the ArvidsonFoto solution. It orchestrates all application components and provides access to the Aspire Dashboard with OpenTelemetry for local and Codespaces development.

## What is Aspire AppHost?

The AppHost is the entry point for running the entire ArvidsonFoto solution with .NET Aspire orchestration. It provides:

- **Aspire Dashboard** - Visual monitoring dashboard at http://localhost:15888
- **OpenTelemetry** - Distributed tracing, metrics, and logs
- **Service Discovery** - Automatic service-to-service communication
- **Health Checks** - Monitor application health
- **Multiple Instances** - Run different configurations simultaneously

## Available Instances

### 1. **arvidsonfoto** (Main Website)
- **Purpose:** Public-facing website
- **Default URL:** `https://localhost:5001` or `http://localhost:5000`
- **Features:**
  - Photo gallery
  - Guestbook
  - Contact forms
  - Search functionality

### 2. **arvidsonfoto-admin** (Admin & API Portal)
- **Purpose:** Development tools, API documentation, and admin panel
- **Default URL:** `https://localhost:7001` or `http://localhost:7000`
- **Access Points:**
  - `/dev` or `/admin` - Landing page with quick links
  - `/scalar/v1` - Scalar API Documentation (interactive)
  - `/openapi/v1.json` - OpenAPI schema (JSON)
  - `/UploadAdmin` - Admin panel for image management
  - `/UploadAdmin/RedigeraBilder` - Edit images
  - `/UploadAdmin/HanteraGB` - Manage guestbook

## Quick Start

### Prerequisites

1. **.NET 10 SDK** (already installed in this project)
2. **Docker Desktop** (optional, only needed if using SQL Server container)

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
1. Start both ArvidsonFoto instances (main website and admin portal)
2. Open the Aspire Dashboard in your browser
3. Configure In-Memory database for both instances

### Aspire Dashboard

Once running, the Aspire Dashboard will be available at:
- **Dashboard URL**: http://localhost:15888 (or https://localhost:19234)

The dashboard provides:
- **Resources** - Status of both website instances
- **Console Logs** - Real-time logs from both applications
- **Traces** - Distributed tracing for request flows
- **Metrics** - Performance metrics and graphs
- **Endpoints** - Click to open main website or admin portal

## Using in Visual Studio

1. Set `ArvidsonFoto.AppHost` as the startup project
2. Press **F5** or click **Start Debugging**
3. The Aspire Dashboard will open automatically
4. Click on the endpoint links to open:
   - Main website (arvidsonfoto)
   - Admin & API portal (arvidsonfoto-admin)

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
4. Click on the forwarded ports to access:
   - Aspire Dashboard
   - Main website
   - Admin portal

## Port Configuration

| Service | HTTPS | HTTP | Purpose |
|---------|-------|------|---------|
| Main Website (arvidsonfoto) | 5001 | 5000 | Public website |
| Admin Portal (arvidsonfoto-admin) | 7001 | 7000 | API docs & Admin |
| Aspire Dashboard | 19234 | 15888 | Monitoring |

## Configuration

### AppHost.cs

The main configuration file that defines all resources:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Main public-facing website
var arvidsonFoto = builder.AddProject<Projects.ArvidsonFoto>("arvidsonfoto")
    .WithExternalHttpEndpoints();

// Admin & API portal on different ports
var arvidsonFotoAdmin = builder.AddProject<Projects.ArvidsonFoto>("arvidsonfoto-admin")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ASPNETCORE_URLS", "https://localhost:7001;http://localhost:7000")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

builder.Build().Run();
```

### Admin Portal Landing Page

Navigate to `/dev` or `/admin` on the admin instance (https://localhost:7001/dev) to see:
- Direct links to Scalar API documentation
- Links to all admin features
- Environment status
- Quick navigation

## Development Workflow

### Testing the API

1. Start Aspire AppHost
2. Click on **arvidsonfoto-admin** endpoint in dashboard
3. Navigate to `/scalar/v1`
4. Explore and test all API endpoints interactively

### Admin Work

1. Start Aspire AppHost
2. Click on **arvidsonfoto-admin** endpoint
3. Navigate to `/admin` for landing page
4. Click on desired admin feature (image management, guestbook, etc.)
5. Login if required

### Regular Website Testing

1. Start Aspire AppHost
2. Click on **arvidsonfoto** endpoint in dashboard
3. Browse the public-facing website
4. Test gallery, guestbook, contact forms, etc.

## OpenTelemetry Integration

### Traces

View distributed traces in the dashboard:
1. Navigate to **Traces** tab
2. Filter by service (arvidsonfoto or arvidsonfoto-admin)
3. Click on a trace to see the full request flow
4. Analyze performance bottlenecks

### Metrics

Available metrics for both instances:
- **HTTP metrics**: Request count, duration, status codes
- **Runtime metrics**: CPU usage, memory, garbage collection
- **.NET metrics**: Thread pool, exceptions, etc.

### Logs

Structured logging with:
- **Console output** - Available in Development mode
- **File logging** - `logs/appLog.txt` (rotates daily)
- **Dashboard view** - Centralized logs from both instances
- **Filtering** by level, service, or search terms

## Database Configuration

Both instances use **In-Memory Database** in Development:
- No Docker or SQL Server installation needed
- Data is seeded automatically on startup
- Data persists only while the application runs
- Perfect for testing and development

To use SQL Server instead:
1. Set `UseInMemoryDatabase` to `false` in appsettings.Development.json
2. Add SQL Server container to AppHost.cs
3. Reference the database in both project instances

## Troubleshooting

### Port Already in Use

```
Error: Address already in use
```

**Solution**: 
- Stop other applications using the configured ports
- Or change ports in AppHost.cs:

```csharp
.WithEnvironment("ASPNETCORE_URLS", "https://localhost:8001;http://localhost:8000")
```

### Admin Portal Not Accessible

**Solution**:
- Ensure you're using the correct port (7001 for HTTPS, 7000 for HTTP)
- Check Aspire Dashboard for the correct endpoint URL
- Verify that ASPNETCORE_ENVIRONMENT is set to "Development"

### API Documentation Not Showing

**Solution**:
- OpenAPI is only available in Development mode
- Navigate to the admin instance (port 7001), not the main website (port 5001)
- Ensure you're accessing `/scalar/v1` on the admin portal

## Benefits of Dual Instances

### Separation of Concerns
- ✅ Public website isolated from admin/API traffic
- ✅ Different ports prevent accidental exposure
- ✅ Can apply different security policies

### Development Efficiency
- ✅ Test API and admin features without affecting public site
- ✅ Dedicated instance for API development
- ✅ Easy to demonstrate features to stakeholders

### Production-Like Setup
- ✅ Mimics microservices architecture
- ✅ Prepares for eventual service separation
- ✅ Better observability practices

## Service Defaults

The `ArvidsonFoto.ServiceDefaults` project provides:
- OpenTelemetry configuration
- Health check defaults
- Service discovery
- Resilience patterns (retry, circuit breaker)

Both instances automatically include ServiceDefaults:

```csharp
builder.AddServiceDefaults();
app.MapDefaultEndpoints();
```

## Learn More

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Aspire Dashboard](https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard)
- [Scalar API Documentation](https://github.com/scalar/scalar)
- [OpenTelemetry](https://opentelemetry.io/)

## Next Steps

1. **Start the AppHost** - Run it and explore both instances
2. **Test the API** - Use Scalar on the admin portal
3. **Manage Content** - Use admin features on port 7001
4. **Browse the Site** - Use the public website on port 5001
5. **Monitor Everything** - Use Aspire Dashboard for all instances

---

**Note**: This configuration uses .NET Aspire with dual instances for maximum development flexibility. Both instances share the same codebase but serve different purposes.
