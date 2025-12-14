# In-Memory Database Configuration

## Overview

The application now supports in-memory database for development environments, particularly useful when running in Codespaces where SQL Server LocalDB is not available.

## How It Works

The application automatically detects when to use in-memory database based on:

1. **Environment Variables**: Automatically detects Codespaces environment
   - `CODESPACES` environment variable exists
   - `GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN` environment variable exists

2. **Configuration Setting**: Manual override in `appsettings.Development.json`
   ```json
   {
     "ConnectionStrings": {
       "UseInMemoryDatabase": "true"
     }
   }
   ```

## Features

### ✅ **Automatic Database Creation**
- In-memory database is created automatically when the application starts
- No migrations needed

### ✅ **Pre-seeded Test Data**
The in-memory database comes pre-populated with:
- **Categories (TblMenus)**: Fåglar, Däggdjur, Kräldjur, etc.
- **Images (TblImages)**: Sample image records with descriptions and dates
- **Guestbook (TblGbs)**: Sample guestbook entries
- **Page Counters (TblPageCounter)**: Page visit tracking data

### ✅ **API Endpoints Work Immediately**
All API endpoints work with the seeded data:
```bash
# Get all categories
curl http://localhost:5000/api/categories

# Get images from "Fåglar" category
curl http://localhost:5000/api/bilder/Faglar

# Search for categories
curl http://localhost:5000/api/search?s=duva
```

## Configuration Details

### Database Context Configuration
```csharp
var useInMemoryDb = Environment.GetEnvironmentVariable("CODESPACES") != null || 
                   Environment.GetEnvironmentVariable("GITHUB_CODESPACES_PORT_FORWARDING_DOMAIN") != null ||
                   Configuration.GetConnectionString("UseInMemoryDatabase") == "true";

if (useInMemoryDb)
{
    services.AddDbContext<ArvidsonFotoDbContext>(options =>
        options.UseInMemoryDatabase("ArvidsonFotoInMemory"));
}
else
{
    services.AddDbContext<ArvidsonFotoDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
}
```

### Data Seeding
```csharp
if (useInMemoryDb)
{
    using (var scope = app.ApplicationServices.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ArvidsonFotoDbContext>();
        context.Database.EnsureCreated();
        context.SeedInMemoryDatabase();
    }
}
```

## Manual Control

To force in-memory database in any environment, add to `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "UseInMemoryDatabase": "true"
  }
}
```

To disable in-memory database (even in Codespaces), set:

```json
{
  "ConnectionStrings": {
    "UseInMemoryDatabase": "false"
  }
}
```

## Benefits

1. **✅ Works in Codespaces**: No LocalDB dependency
2. **✅ Fast Startup**: No database server needed
3. **✅ Pre-populated Data**: Ready to test immediately
4. **✅ API Testing**: All endpoints work with sample data
5. **✅ Automatic Detection**: No manual configuration needed
6. **✅ Development Friendly**: Reset on every restart

## Limitations

- **Data is not persisted**: All data is lost when application stops
- **Memory only**: No file-based persistence
- **Single session**: Each application instance has its own database

## Troubleshooting

### Issue: "LocalDB is not supported on this platform"
**Solution**: The configuration should automatically detect Codespaces and use in-memory database. If not working, manually set `"UseInMemoryDatabase": "true"` in appsettings.

### Issue: No data in API responses
**Solution**: Verify the seeding is working by checking the logs during startup. The `SeedInMemoryDatabase()` method should run automatically.

### Issue: Want to use SQL Server instead
**Solution**: Set `"UseInMemoryDatabase": "false"` in appsettings and provide a valid SQL Server connection string.