![Last commit](https://img.shields.io/github/last-commit/pownas/ArvidsonFoto-MVC-NET6?style=flat-square&cacheSeconds=86400)

# ArvidsonFoto-MVC-NET6
 Ombyggnation av ArvidsonFoto med MVC och .NET5 (uppgraderad till .NET6)

## Instruktion för att starta webbsidan lokalt
För att starta webbsidan så är det några steg man behöver genomföra. 
1. Kommentera bort ```modelBuilder.InitialDatabaseSeed();``` (ca rad 163) i **[/ArvidsonFoto/Data/ArvidsonFotoDbContext.cs](https://github.com/pownas/ArvidsonFoto-MVC-NET6/blob/main/ArvidsonFoto/Data/ArvidsonFotoDbContext.cs#L163)** , för att kunna skapa en ny databas med dess tillhörande data. 
2. Kör entityframework databas uppdateringar: 
```dotnet ef database update --context ArvidsonFotoDbContext```  
```dotnet ef database update --context ArvidsonFotoIdentityContext```
3. Vill du skapa nya användare för att komma åt: **https://localhost:44300/UploadAdmin**, så behöver du kommentera tillbaka all kod på sidan: **[/ArvidsonFoto/Areas/Identity/Pages/Account/Register.cshtml](https://github.com/pownas/ArvidsonFoto-MVC-NET6/blob/main/ArvidsonFoto/Areas/Identity/Pages/Account/Register.cshtml)**
4. Nu kan du registrera nya användare och sedan logga in på sidan **/UploadAdmin** också. 


## Skapa nya Migrations
För att skapa någon ny migration om en data-modell ändras på, kör kommandot: 
```dotnet ef migrations add DatabaseSeed --context ArvidsonFotoDbContext```



## Systemdokumentation
![ArvidsonFoto](https://github.com/pownas/ArvidsonFoto-MVC-NET6/blob/main/docs/Anvandningsfalls-modell-version1.0-2021-01-27.jpg?raw=true)
