using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

#pragma warning disable CA1303 // Do not pass literals as localized parameters
#pragma warning disable CA1308 // Normalize strings to uppercase

// 1. Definitiera sökvägar i utdatamappen
string lockFilesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProjectLockfiles");
string propsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Directory.Packages.props");
var extractedPackages = new List<NuGetPackage>();

// 2. Kontrollera om mappen med lock-filer existerar
if (Directory.Exists(lockFilesFolder))
{
    // Hitta alla packages.lock.json i undermapparna
    var lockFiles = Directory.GetFiles(lockFilesFolder, "packages.lock.json", SearchOption.AllDirectories);

    foreach (var file in lockFiles)
    {
        // Hämta namnet på projektet baserat på mappnamnet
        string projectName = Path.GetFileName(Path.GetDirectoryName(file) ?? "Okänt Projekt");
        Console.WriteLine($"Läser in lock-fil för: {projectName}");

        string jsonContent = await File.ReadAllTextAsync(file).ConfigureAwait(true);
        var lockFile = JsonSerializer.Deserialize<LockFile>(jsonContent);

        if (lockFile?.Dependencies is null) continue;

        foreach (var framework in lockFile.Dependencies)
        {
            foreach (var package in framework.Value)
            {
                if (package.Value.Type == "Project" || string.IsNullOrEmpty(package.Value.ContentHash))
                    continue; // Skippa interna projekt

                // Lägg till i listan (om du vill undvika dubbletter mellan projekt kan du lägga till en Distinct-kontroll här)
                if (!extractedPackages.Any(p => p.Name.Equals(package.Key, StringComparison.OrdinalIgnoreCase) && p.Version == (package.Value.Resolved ?? "Okänd")))
                {
                    extractedPackages.Add(new NuGetPackage(
                        package.Key,
                        package.Value.Resolved ?? "Okänd",
                        package.Value.ContentHash));
                }
            }
        }
    }
}
else
{
    Console.WriteLine("Hittade inga projektfiler i ProjectLockfiles. Kör en Build på lösningen först.");
    return;
}

// 3. Läs in Directory.Packages.props
if (!File.Exists(propsFilePath))
{
    Console.WriteLine("Kunde inte hitta Directory.Packages.props i utdatamappen.");
    return;
}

var doc = XDocument.Load(propsFilePath);
var centralPackages = doc.Descendants("PackageVersion")
    .Select(node => new
    {
        Id = node.Attribute("Include")?.Value ?? node.Attribute("Update")?.Value ?? "",
        Version = node.Attribute("Version")?.Value ?? ""
    })
    .Where(p => !string.IsNullOrWhiteSpace(p.Id))
    .ToList();

// Konfigurera HttpClient att automatiskt hantera GZIP/Deflate-komprimering
using var handler = new HttpClientHandler
{
    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
};
using var client = new HttpClient(handler);

Console.WriteLine("Hämtar Service Index från NuGet...");

// 4. Hämta NuGet V3 Service Index
var serviceIndexUrl = "https://api.nuget.org/v3/index.json";
var serviceIndex = await client.GetFromJsonAsync<ServiceIndex>(serviceIndexUrl).ConfigureAwait(true);

// Hämta RegistrationsBaseUrl för att kunna kolla senaste versioner live
var regResourceUrl = serviceIndex?.Resources
    .FirstOrDefault(r => r.Type.StartsWith("RegistrationsBaseUrl/3.6.0", StringComparison.OrdinalIgnoreCase))?.Id;

// 5. Hitta slutpunkten för VulnerabilityInfo i indexet
var vulnResourceUrl = serviceIndex?.Resources
    .FirstOrDefault(r => r.Type.StartsWith("VulnerabilityInfo", StringComparison.OrdinalIgnoreCase))?.Id;

if (vulnResourceUrl is null || regResourceUrl is null)
{
    Console.WriteLine("Kunde inte hitta nödvändiga NuGet-API-slutpunkter.");
    return;
}

// 6. Hämta fillistan
var vulnFiles = await client.GetFromJsonAsync<List<VulnerabilityFile>>(vulnResourceUrl).ConfigureAwait(true);

// 7. Definiera den saknade variabeln
string? baseJsonUrl = vulnFiles?.FirstOrDefault(f => f.Name == "base")?.Id;
if (baseJsonUrl is null)
{
    Console.WriteLine("Kunde inte hitta base.json.");
    return;
}

Console.WriteLine($"Laddar ner sårbarhetsdatabasen från: {baseJsonUrl}");

// 8. Ladda ner sårbarhetsdatabasen
var vulnerabilities = await client.GetFromJsonAsync<Dictionary<string, List<Vulnerability>>>(baseJsonUrl).ConfigureAwait(true);

// 9. Scanna dina extraherade paket i minnet
Console.WriteLine("\nPåbörjar skanning...");
foreach (var pkg in extractedPackages)
{
    string searchKey = pkg.Name.ToLowerInvariant();

    // --- Kolla status i Central Package Management (CPM) ---
    var cpmMatch = centralPackages.FirstOrDefault(c => c.Id.Equals(pkg.Name, StringComparison.OrdinalIgnoreCase));
    string cpmStatus = cpmMatch != null
        ? $"Ja (Definierad till: {cpmMatch.Version})"
        : "Nej (Transitivt beroende)";

    // --- Hämta absolut senaste versionen live från NuGet ---
    string latestNuGetVersion = "Okänd";
    try
    {
        // Slå upp paketet i NuGets registreringsindex (alltid i gemener)
        var packageRegUrl = $"{regResourceUrl.TrimEnd('/')}/{searchKey}/index.json";
        var regIndex = await client.GetFromJsonAsync<NugetRegistrationIndex>(packageRegUrl).ConfigureAwait(true);

        if (regIndex?.Pages != null && regIndex.Pages.Count > 0)
        {
            // Ta sista sidan, och sista paketet på den sidan (vilket är det nyaste)
            var lastPage = regIndex.Pages[^1];
            if (lastPage.Items != null && lastPage.Items.Count > 0)
            {
                latestNuGetVersion = lastPage.Items[^1].CatalogEntry.Version;
            }
        }
    }
    catch
    {
        // Om paketet inte hittas eller timeout sker, låt den förbli "Okänd"
    }

    // --- SKRIV UT RESULTATET ---
    if (vulnerabilities!.TryGetValue(searchKey, out var vulnList))
    {
        Console.WriteLine(new string('-', 60));
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[VARNING] Sårbarhet funnen i: {pkg.Name}");
        Console.ResetColor();

        Console.WriteLine($"  -> Nuvarande installerad version:  {pkg.Version}");
        Console.WriteLine($"  -> Hanteras centralt i CPM?         {cpmStatus}");
        if (latestNuGetVersion != "Okänd" && latestNuGetVersion != pkg.Version)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"  -> Senaste live-version på NuGet:  {latestNuGetVersion}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  -> Senaste live-version på NuGet:  {latestNuGetVersion}");
        }
        Console.ResetColor();
        Console.WriteLine($"  -> Hittade {vulnList.Count} sårbarhetsintervall:");

        foreach (var vuln in vulnList)
        {
            var severityStr = vuln.Severity switch
            {
                0 => "Låg",
                1 => "Medel",
                2 => "Hög",
                3 => "Kritisk",
                _ => "Okänd"
            };

            Console.WriteLine($"      - Allvarlighet: [{severityStr}] | Berör: {vuln.Versions}");
            Console.WriteLine($"        Länk: {vuln.Url}");
        }
        Console.WriteLine(new string('-', 60));
    }
    else
    {
        // Även om paketet är friskt kan det vara bra att se om det är utdaterat
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("[OK] ");
        Console.ResetColor();
        Console.Write($"{pkg.Name} (Version: {pkg.Version}) | CPM: {cpmStatus} ");

        if (latestNuGetVersion != "Okänd" && latestNuGetVersion != pkg.Version)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"[Nyare version finns: {latestNuGetVersion}]");
            Console.ResetColor();
        }
        Console.WriteLine();
    }
}

#region DataModeller

record ServiceIndex(
    [property: JsonPropertyName("resources")] List<Resource> Resources
);

record Resource(
    [property: JsonPropertyName("@id")] string Id,
    [property: JsonPropertyName("@type")] string Type
);

record VulnerabilityFile(
    [property: JsonPropertyName("@name")] string Name,
    [property: JsonPropertyName("@id")] string Id
);

record Vulnerability(
    [property: JsonPropertyName("severity")] int Severity,
    [property: JsonPropertyName("versions")] string Versions,
    [property: JsonPropertyName("url")] string Url
);

record LockFile(
    [property: JsonPropertyName("dependencies")] Dictionary<string, Dictionary<string, LockDependency>> Dependencies
);

record LockDependency(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("resolved")] string Resolved,
    [property: JsonPropertyName("contentHash")] string ContentHash
);

record NuGetPackage(string Name, string Version, string ContentHash);

record NugetRegistrationIndex(
    [property: JsonPropertyName("items")] List<RegistrationPage> Pages
);

record RegistrationPage(
    [property: JsonPropertyName("items")] List<RegistrationItem> Items
);
record RegistrationItem(
    [property: JsonPropertyName("catalogEntry")] CatalogEntry CatalogEntry
);
record CatalogEntry(
    [property: JsonPropertyName("version")] string Version
);

#endregion
