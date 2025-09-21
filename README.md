![Last commit](https://img.shields.io/github/last-commit/pownas/ArvidsonFoto-MVC-NET8?style=flat-square&cacheSeconds=86400)

# ArvidsonFoto-MVC .NET web
Ombyggnation av ArvidsonFoto med MVC och .NET (uppgraderad från .NET5 till .NET6 till .NET8 till .NET9... osv. till senaste .NET)
  
  
## Instruktion för att starta webbsidan lokalt
För att starta webbsidan så är det några steg man behöver genomföra. 
1. Kommentera bort ```modelBuilder.InitialDatabaseSeed();``` (ca rad 163) i **[/ArvidsonFoto/Data/ArvidsonFotoDbContext.cs](https://github.com/pownas/ArvidsonFoto-MVC-NET8/blob/main/ArvidsonFoto/Data/ArvidsonFotoDbContext.cs#L163)** , för att kunna skapa en ny databas med dess tillhörande data. 
2. Kör entityframework databas uppdateringar: 
```dotnet ef database update --context ArvidsonFotoDbContext```  
```dotnet ef database update --context ArvidsonFotoIdentityContext```
3. Vill du skapa nya användare för att komma åt: **https://localhost:44300/UploadAdmin**, så behöver du kommentera tillbaka all kod på sidan: **[/ArvidsonFoto/Areas/Identity/Pages/Account/Register.cshtml](https://github.com/pownas/ArvidsonFoto-MVC-NET8/blob/main/ArvidsonFoto/Areas/Identity/Pages/Account/Register.cshtml)**
4. Nu kan du registrera nya användare och sedan logga in på sidan **/UploadAdmin** också. 
  
  
## Skapa nya Migrations
För att skapa någon ny migration om en data-modell ändras på, kör kommandot: 
```dotnet-ef migrations add DatabaseSeed --context ArvidsonFotoDbContext```
  
  
## Fel vid körning med EF-core
Om du får ett felmeddelande när du kör: ```dotnet-ef database update```, som säger:  
```
Could not execute because the specified command or file was not found.
Possible reasons for this include: 
  * You misspelled a build-in dotnet command.
  * You intended to execute a .NET program , but dotnet-ef does not exist.
  * You intended to run a global tool, but dotnet-prefixed executable with this name could not be found on the PATH.
```  
  
Då behöver du installera dotnet-ef CLI (dotnet entity framework), som installeras via kommandot:  
```dotnet tool install --global dotnet-ef```

## Uppdatera entityframework till senaste versionen
Uppdatera din dotnet-ef till version 10.0.0 eller högre med kommandot:  
```dotnet tool update --global dotnet-ef```

Eller via CURL: 
```sh
  Write-Host "Installing .NET 10 SDK using Microsoft's installation script..."
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version latest --channel 10.0
    
  Write-Host "Configuring PATH for .NET..."
  export PATH="$HOME/.dotnet:$PATH"
  echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.bashrc
```

## Installera developer-certifikat
Om du får felmeddelande när du kör webbsidan lokalt, som säger:  
```
Unable to configure HTTPS endpoint. No server certificate was specified, and the default developer certificate could not be found.
```
Då behöver du installera ett developer-certifikat med kommandot:  
```ps
# Rensa bort alla gamla certifikat
dotnet dev-certs https --clean
# Installera ASP.NET Core HTTPS development certifikat
dotnet dev-certs https --trust
```


## Systemdokumentation
![ArvidsonFoto](https://github.com/pownas/ArvidsonFoto-MVC-NET-web/blob/main/docs/Anvandningsfalls-modell-version1.0-2021-01-27.jpg?raw=true)
