# ArvidsonFoto Database Konfiguration

## Nuvarande Status

Din applikation är redan korrekt konfigurerad för att använda **in-memory database** med testdata!

### Vad har gjorts

1. ✅ **ArvidsonFotoCoreDbSeeder.cs** - Innehåller all seed-data:
   - 2 gästboksinlägg
   - 543 kategorier (menyer)
   - 44 sidräknare
   - 100 bilder

2. ✅ **Program.cs** - Seedar in-memory databasen automatiskt när applikationen startar:
   ```csharp
   if (useInMemoryDb)
   {
       using (var scope = app.Services.CreateScope())
       {
           var coreContext = scope.ServiceProvider.GetRequiredService<ArvidsonFotoCoreDbContext>();
           coreContext.Database.EnsureCreated();
           
           // Seeda data om databasen är tom
           if (!coreContext.TblImages.Any())
           {
               Log.Information("Seeding in-memory database with test data...");
               coreContext.SeedInMemoryDatabase();
               Log.Information("In-memory database seeded successfully...");
           }
       }
   }
   ```

3. ✅ **appsettings.Development.json** - UseInMemoryDatabase är satt till "true"

### När du kör applikationen

Kör applikationen via Aspire AppHost:
```bash
dotnet run --project ArvidsonFoto.AppHost
```

Du kommer att se följande i loggen:
```
Seeding in-memory database with test data...
In-memory database seeded successfully with 100 images, 543 categories, 2 guestbook entries
```

Applikationen kommer nu ha testdata tillgänglig på:
- **Startsida**: Visar 12 senaste bilderna
- **Kategorier**: 543 kategorier tillgängliga i menyn
- **Gästbok**: 2 testinlägg
- **Bilder**: 100 testbilder

---

## Alternativ: Använd SQL Server istället

Om du vill använda en riktig SQL Server databas istället, gör följande:

### Steg 1: Uppdatera AppHost.cs

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Lägg till SQL Server container
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume() // Persisterar data mellan omstarter
    .AddDatabase("arvidsonfoto-db");

var arvidsonFoto = builder.AddProject<Projects.ArvidsonFoto>("arvidsonfoto")
    .WithReference(sqlServer) // Injicerar connection string automatiskt
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

### Steg 2: Uppdatera appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "UseInMemoryDatabase": "false"
  }
}
```

### Steg 3: Kör migrations

```bash
cd ArvidsonFoto
dotnet ef database update
```

### Steg 4: Seeda SQL Server med data

Du kan antingen:

**Alternativ A**: Använd OnModelCreating i ArvidsonFotoCoreDbContext:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Seeda data via migrations
    modelBuilder.InitialDatabaseSeed();
}
```

**Alternativ B**: Kör ett seed-script vid uppstart:
```csharp
// I Program.cs ConfigureMiddleware
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ArvidsonFotoCoreDbContext>();
    context.Database.Migrate(); // Kör migrations
    
    // Seeda om tom
    if (!context.TblImages.Any())
    {
        context.SeedInMemoryDatabase(); // Funkar även för SQL Server
    }
}
```

---

## Felsökning

### Ser ingen data när applikationen körs?

1. Kontrollera loggen för meddelandet: `"Seeding in-memory database with test data..."`
2. Om meddelandet inte visas, kontrollera att `UseInMemoryDatabase` är `"true"` i appsettings.Development.json
3. Kontrollera att Aspire AppHost körs och inte bara ArvidsonFoto projektet direkt

### Vill se fler bilder?

Lägg till fler bilder i `ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image` listan.

### Behöver exportera mer data från SQL Server?

Använd SQL query:erna i kommentarerna i `ArvidsonFotoCoreDbSeeder.cs` för att exportera data från din SQL Server databas.

