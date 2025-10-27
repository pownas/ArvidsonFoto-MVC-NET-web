![Last commit](https://img.shields.io/github/last-commit/pownas/ArvidsonFoto-MVC-NET8?style=flat-square&cacheSeconds=86400)

# ArvidsonFoto-MVC .NET web
Ombyggnation av ArvidsonFoto med MVC och .NET (uppgraderad från .NET5 till .NET6 till .NET8 till .NET9... osv. till senaste .NET)

## Funktioner

- 📸 Bildgalleri med kategorier
- 🌍 **Flerspråksstöd** - Svenska och Engelska (se [Lokalisering](#lokalisering))
- 📝 Gästbok
- 🔍 Bildsökning
- 📧 Kontaktformulär
- 👤 Användarhantering med Identity
- 📊 Admin-gränssnitt för bilduppladdning

## Lokalisering

Webbplatsen stöder nu flerspråkighet med svenska (sv-SE) och engelska (en-US). Användare kan växla språk via en språkväljare i navigeringsmenyn.

För mer information om hur lokaliseringen fungerar och hur man lägger till nya översättningar, se [LOCALIZATION.md](docs/LOCALIZATION.md).
  
  
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

## Diagram och Beskrivningar

Detta dokument innehåller två olika diagram som beskriver funktionaliteten och arkitekturen i projektet **ArvidsonFoto-MVC-NET-web**. Diagrammen är skapade för att ge en överblick av användningsfall och tekniska flöden i applikationen.

---

### 1. Aktörer och Användningsfall (Use-case)

**Beskrivning**
Detta dokument innehåller två olika diagram som beskriver funktionaliteten och arkitekturen i projektet **ArvidsonFoto-MVC-NET-web**. Diagrammen är skapade för att ge en överblick av användningsfall och tekniska flöden i applikationen.

Detta flödesschema visualiserar de olika aktörerna i systemet och deras interaktion med olika funktionella mål (use cases).

```mermaid
flowchart TD
  subgraph Aktörer
    A1[Besökare]
    A2[Registrerad användare]
    A3[Fotograf]
    A4[Redaktör]
    A5[System]
  end

  subgraph "Use cases"
    UC1(Visa galleri)
    UC2(Sök / filtrera)
    UC3(Visa bilddetaljer)
    UC4(Logga in)
    UC5(Logga ut)
    UC6(Ladda upp bild)
    UC7(Kommentera bild)
    UC8(Ladda ner bild)
    UC9(Redigera metadata)
    UC10(Ta bort bild)
    UC11(Granska och publicera)
    UC12(Generera thumbnails)
    UC13(Indexera metadata)
    UC14(Optimera bilder)
  end

  A1 --> UC1
  A1 --> UC2
  A1 --> UC3

  A2 --> UC4
  A2 --> UC5
  A2 --> UC6
  A2 --> UC7
  A2 --> UC8

  A3 --> UC6
  A3 --> UC9
  A3 --> UC10

  A4 --> UC11

  A5 --> UC12
  A5 --> UC13
  A5 --> UC14
```

### 2. Flödesschema: Bilduppladdning till Publicering

**Beskrivning**

Detta diagram visar det tekniska flödet för en bilduppladdning, från användarens gränssnitt till lagring och bearbetning i bakgrunden, och slutligen publicering och visning via CDN. Flödet innefattar steg som validering, lagring, jobbköer och bakgrundsprocesser.

```mermaid
flowchart LR
User[Registrerad användare / Fotograf] -->|Laddar upp bild via formulär| WebApp[ASP.NET MVC - Upload Controller]
WebApp -->|Validera och spara metadata| DB[SQL Database]
WebApp -->|Spara originalfil| Storage[Fil-lagring eller Blob]
WebApp -->|Enqueue jobb| Queue[Jobbkö eller Background Queue]
Queue --> Worker[Background worker eller Image processor]
Worker -->|Generera thumbnails och olika storlekar| Storage
Worker -->|Optimera eller konvertera bilder| Storage
Worker -->|Extrahera och uppdatera metadata, EXIF och taggar| DB
Worker -->|Markera som publicerad eller uppdatera status| DB
Worker -->|Pusha eller uppdatera CDN cache| CDN[CDN eller Cache]
Worker -->|Skicka notifikation valfritt| Notifier[Notifiering eller E-post eller UI-flagga]
WebApp -->|Visa publicerad bild via CDN| Visitor[Besökare eller Sök etc.]
Storage -->|Servera bild| CDN
DB -->|Används för sökning och visning| WebApp
```

