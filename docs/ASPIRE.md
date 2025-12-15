# .NET Aspire Integration för ArvidsonFoto

## ✅ Aspire är nu aktiverat!

**Goda nyheter!** Projektet inkluderar nu fullt fungerande Aspire AppHost med:
- ✨ **Aspire Dashboard v13** med OpenTelemetry
- ✨ **SQL Server orchestration** i Docker
- ✨ **Distributed tracing, metrics & logs**
- ✨ **Health checks** och **Service discovery**

## Snabbstart

### Kör Aspire AppHost

```bash
# Steg 1: Starta Docker Desktop
# Ladda ner från https://www.docker.com/products/docker-desktop/

# Steg 2: Kör AppHost från projektets rot
dotnet run --project ArvidsonFoto.AppHost
```

Aspire Dashboard öppnas automatiskt på: **http://localhost:15888**

Se [ArvidsonFoto.AppHost/README.md](../ArvidsonFoto.AppHost/README.md) för mer information.

---

## Vad är .NET Aspire?

.NET Aspire är ett ramverk för att bygga observerbara, produktionsklara molnbaserade applikationer med .NET. Det förenklar lokal utveckling och debugging genom att tillhandahålla en orkestrator för att hantera flera tjänster, databaser och resurser.

## Varför .NET Aspire?

- **Förenklad lokal utveckling**: Kör hela applikationen med alla dependencies (databas, cache, etc.) med ett kommando
- **Orchestration**: Hantera flera tjänster och deras dependencies enkelt
- **Observability**: Inbyggd support för distributed tracing, metrics och logging
- **Service Discovery**: Automatisk service discovery mellan applikationer
- **Development Dashboard**: Visuell dashboard för att övervaka tjänster och resources

## Förutsättningar

- .NET 10 SDK eller senare (redan installerat) ✅
- Docker Desktop (för att köra resurser som SQL Server, Redis, etc.)
- Visual Studio 2022 17.13+ eller Visual Studio Code med C# Dev Kit

**OBS**: Aspire workload är **inte längre nödvändig**. Aspire v13 fungerar via NuGet-paket istället.

## Vad ingår i projektet?

### 1. ArvidsonFoto.AppHost ✅

Orkestrator-projektet som startar hela lösningen:
- Konfigurerar SQL Server i Docker
- Startar ArvidsonFoto webbapplikationen
- Exponerar Aspire Dashboard
- Hanterar dependencies mellan tjänster

**Kod**: Se `ArvidsonFoto.AppHost/AppHost.cs`

### 2. ArvidsonFoto.ServiceDefaults ✅

Gemensamma servicekonfigurationer:
- OpenTelemetry (tracing, metrics, logs)
- Health checks (/health, /alive)
- Service discovery
- Resilience patterns (retry, circuit breaker)

**Kod**: Se `ArvidsonFoto.ServiceDefaults/Extensions.cs`

### 3. ArvidsonFoto - Uppdaterat ✅

Huvudapplikationen använder nu ServiceDefaults:
- `builder.AddServiceDefaults()` - Lägger till observability
- `app.MapDefaultEndpoints()` - Exponerar health checks

**Kod**: Se `ArvidsonFoto/Program.cs`

## Hur det fungerar

När du kör AppHost:

1. **Docker startar SQL Server** - Automatisk container med persistent data
2. **ArvidsonFoto startar** - Med automatisk connection string
3. **Dashboard öppnas** - Visual monitoring på http://localhost:15888
4. **OpenTelemetry aktiveras** - Traces, metrics och logs samlas in

## Ingen installation behövs (Redan klart!)

~~Du behöver inte längre skapa AppHost eller ServiceDefaults - de finns redan!~~

<details>
<summary>📚 Om du vill lära dig hur det gjordes (klicka för att expandera)</summary>

## Hur projektet konfigurerades

### Steg 1: Skapa AppHost-projekt

```bash
cd /home/runner/work/ArvidsonFoto-MVC-NET-web/ArvidsonFoto-MVC-NET-web
dotnet new aspire-apphost -n ArvidsonFoto.AppHost
```

### Steg 2: Skapa ServiceDefaults-projekt

```bash
dotnet new aspire-servicedefaults -n ArvidsonFoto.ServiceDefaults
```

### Steg 3: Uppdatera ArvidsonFoto.csproj

Lägg till referens till ServiceDefaults-projektet:

```xml
<ItemGroup>
  <ProjectReference Include="../ArvidsonFoto.ServiceDefaults/ArvidsonFoto.ServiceDefaults.csproj" />
</ItemGroup>
```

✅ **Redan gjort i projektet!**

### Steg 4: Uppdatera Program.cs i ArvidsonFoto

Lägg till Aspire service defaults i `Main`-metoden innan `ConfigureServices` anropas:

```csharp
public static void Main(string[] args)
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.File("logs\\appLog.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    try
    {
        Log.Information("Starting web application");
        
        var builder = WebApplication.CreateBuilder(args);

        // Add Aspire service defaults (observability, health checks, resilience)
        builder.AddServiceDefaults();

        // Add services to the container
        ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

        var app = builder.Build();

        // Configure the HTTP request pipeline
        ConfigureMiddleware(app, app.Environment, app.Configuration);

        app.Run();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Application terminated unexpectedly");
        throw;
    }
    finally
    {
        Log.CloseAndFlush();
    }
}
```

Och i `ConfigureMiddleware`-metoden, lägg till:

```csharp
private static void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env, IConfiguration configuration)
{
    // ... existing seeding code ...

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
        app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
    }
    
    // ... rest of middleware ...
    
    // Map default endpoints (health checks, etc.) - add before MapControllerRoute
    app.MapDefaultEndpoints();
    
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();
    
    // ... rest of code
}
```

### Steg 5: Konfigurera AppHost

Redigera `ArvidsonFoto.AppHost/Program.cs`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Lägg till SQL Server resursen
var sqlServer = builder.AddSqlServer("sql")
    .WithDataVolume()
    .AddDatabase("ArvidsonFotoDb");

// Lägg till huvudapplikationen
var arvidsonFoto = builder.AddProject<Projects.ArvidsonFoto>("arvidsonfoto")
    .WithReference(sqlServer)
    .WithExternalHttpEndpoints();

// Lägg till LogReader-applikationen (om den ska användas)
// var logReader = builder.AddProject<Projects.ArvidsonFoto_LogReader>("logreader")
//     .WithReference(arvidsonFoto);

var app = builder.Build();
app.Run();
```

## Starta applikationen med Aspire

### Via kommandoraden

```bash
# Navigera till AppHost-projektet
cd ArvidsonFoto.AppHost

# Kör Aspire Orchestrator
dotnet run
```

Detta kommer att:
1. Starta SQL Server i en Docker-container
2. Starta ArvidsonFoto-webbapplikationen
3. Öppna Aspire Dashboard i din webbläsare (vanligtvis http://localhost:15888)

### Via Visual Studio

1. Sätt `ArvidsonFoto.AppHost` som startup-projekt
2. Tryck F5 för att starta

### Via Visual Studio Code

1. Öppna mappen i VS Code
2. Använd "Run and Debug"-panelen
3. Välj "Aspire App Host" från dropdown-menyn
4. Tryck F5

## Aspire Dashboard

Dashboard visar:
- **Resources**: Alla tjänster och resurser (SQL Server, webbapp, etc.)
- **Console Logs**: Samlade loggar från alla tjänster
- **Traces**: Distributed tracing för att följa requests genom systemet
- **Metrics**: Prestandametrik för varje tjänst

## Avancerad konfiguration

### Lägga till Redis för caching

```csharp
var redis = builder.AddRedis("cache");

var arvidsonFoto = builder.AddProject<Projects.ArvidsonFoto>("arvidsonfoto")
    .WithReference(sqlServer)
    .WithReference(redis)
    .WithExternalHttpEndpoints();
```

I din `Program.cs`:

```csharp
// Lägg till i ConfigureServices
builder.AddRedisOutputCache("cache");
```

### Lägga till Blob Storage för bilder

```csharp
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator()
    .AddBlobs("images");

var arvidsonFoto = builder.AddProject<Projects.ArvidsonFoto>("arvidsonfoto")
    .WithReference(sqlServer)
    .WithReference(storage)
    .WithExternalHttpEndpoints();
```

### Environment-specifik konfiguration

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Använd olika konfiguration baserat på miljö
if (builder.Environment.IsDevelopment())
{
    // Lokal SQL Server i Docker
    var sqlServer = builder.AddSqlServer("sql")
        .WithDataVolume()
        .AddDatabase("ArvidsonFotoDb");
}
else
{
    // Azure SQL Database i produktion
    var sqlServer = builder.AddAzureSqlDatabase("sql", "ArvidsonFotoDb");
}
```

## Migration från befintlig setup

### Databas

Nuvarande setup använder:
- LocalDB för lokal utveckling
- In-memory databas för Codespaces

Med Aspire:
- SQL Server i Docker för lokal utveckling (mer likt produktion)
- Automatisk connection string-hantering
- Enkel övergång till Azure SQL Database

### Connection Strings

Aspire hanterar connection strings automatiskt. Du behöver inte längre hantera olika connection strings för olika miljöer i `appsettings.json`.

```csharp
// Innan (manuell hantering)
var connectionString = configuration.GetConnectionString("DefaultConnection");

// Efter (Aspire hanterar detta automatiskt)
// Connection string injiceras automatiskt via WithReference(sqlServer)
```

## Felsökning

### Docker körs inte

```bash
# Starta Docker Desktop
# Eller på Linux:
sudo systemctl start docker
```

### Port redan används

Dashboard använder ofta port 15888. Om den är upptagen:

```bash
# Stoppa andra processer eller ändra porten i launchSettings.json
```

### SQL Server startar inte

```bash
# Kontrollera Docker logs
docker logs <container-id>

# Eller via Aspire Dashboard - klicka på SQL Server resursen för att se logs
```

## Best Practices

1. **Använd Service Defaults**: De ger dig health checks, metrics och logging gratis
2. **Använd Docker volumes**: För att bevara data mellan körningar
3. **Separera utveckling och test**: Använd olika databasnamn
4. **Övervaka Traces**: Använd dashboard för att förstå performance bottlenecks
5. **Environment Variables**: Använd Aspire för att hantera olika konfigurationer

## Läs mer

- [.NET Aspire Dokumentation](https://learn.microsoft.com/dotnet/aspire/)
- [.NET Aspire GitHub](https://github.com/dotnet/aspire)
- [.NET Aspire Samples](https://github.com/dotnet/aspire-samples)
- [Video: Getting Started with .NET Aspire](https://www.youtube.com/watch?v=z1M-7Bms1Jg)

## Nästa steg

1. Installera Aspire workload
2. Skapa AppHost och ServiceDefaults projekt
3. Uppdatera references och Program.cs
4. Testa lokal utveckling med `dotnet run` från AppHost
5. Utforska Dashboard för observability

---

**Tips**: Börja enkelt med bara SQL Server i Aspire, lägg till fler resurser (Redis, Storage, etc.) när behovet uppstår.
