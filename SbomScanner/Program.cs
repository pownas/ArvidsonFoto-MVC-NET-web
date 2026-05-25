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
                if (!extractedPackages.Any(p => p.Name == package.Key && p.Version == (package.Value.Resolved ?? "Okänd")))
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

// 3. Läs in Directory.Packages.props (Använder nu den säkra sökvägen)
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

// 5. Hitta slutpunkten för VulnerabilityInfo i indexet
var vulnResourceUrl = serviceIndex?.Resources
    .FirstOrDefault(r => r.Type.StartsWith("VulnerabilityInfo", StringComparison.OrdinalIgnoreCase))?.Id;

if (vulnResourceUrl is null)
{
    Console.WriteLine("Kunde inte hitta sårbarhets-API:et.");
    return;
}

// 6. Hämta fillistan (innehåller referenser till base.json)
var vulnFiles = await client.GetFromJsonAsync<List<VulnerabilityFile>>(vulnResourceUrl).ConfigureAwait(true);

// 7. Definiera den saknade variabeln:
string? baseJsonUrl = vulnFiles?.FirstOrDefault(f => f.Name == "base")?.Id;

if (baseJsonUrl is null)
{
    Console.WriteLine("Kunde inte hitta base.json.");
    return;
}

Console.WriteLine($"Laddar ner sårbarhetsdatabasen från: {baseJsonUrl}");

// 8. Ladda ner sårbarhetsdatabasen (base.json)
var vulnerabilities = await client.GetFromJsonAsync<Dictionary<string, List<Vulnerability>>>(baseJsonUrl).ConfigureAwait(true);

// 9. Scanna dina extraherade paket i minnet
Console.WriteLine("\nPåbörjar skanning...");
foreach (var pkg in extractedPackages)
{
    // NuGet-API:et kräver att alla paket-ID:n är i gemener (små bokstäver) vid sökning
    string searchKey = pkg.Name.ToLowerInvariant();

    if (vulnerabilities.TryGetValue(searchKey, out var vulnList))
    {
        ////TODO: Fixa: Här kan vi förbättra logiken för att kontrollera både paketnamn och version
        //if (centralPackages.Any(cpm => cpm.Id.Equals(pkg.Name, StringComparison.OrdinalIgnoreCase) && cpm.Version.Equals(pkg.Version, StringComparison.OrdinalIgnoreCase)))
        //{
        //    Console.WriteLine($"✅ Central Package Management har hanterat uppdateringen av {pkg.Name} till version {centralPackages.First(cpm => cpm.Id.Equals(pkg.Name, StringComparison.OrdinalIgnoreCase) && cpm.Version.Equals(pkg.Version, StringComparison.OrdinalIgnoreCase)).Version} i Directory.Packages.props.");
        //    Console.WriteLine($"OK: Kända sårbarheter finns för {pkg.Name}, hanteras av CPM i Directory.Packages.props, senaste v. {pkg.Version}");
        //}
        //else if (centralPackages.Any(cpm => cpm.Version.Equals(pkg.Version, StringComparison.OrdinalIgnoreCase)))
        //{
        //    Console.WriteLine($"⚠️ OBS: Transitativa paketet {pkg.Name} hanteras av Central Package Management. Men det har inte senaste versionen: {pkg.Version}.");
        //}
        //else
        //{
        //    Console.WriteLine($"⚠️ OBS: Transitativa paketet {pkg.Name} hanteras inte av Central Package Management. Lägg till paketet i CPM och den senaste versionen: {pkg.Version}.");
        //}

        Console.WriteLine($"{Environment.NewLine}[AVVISNING/VARNING] Paket: {pkg.Name} (Använd version: {pkg.Version})");
        Console.WriteLine($" -> Hittade {vulnList.Count} kända sårbarhetsintervall i databasen:");

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

            Console.WriteLine($"    - Allvarlighetsgrad: {severityStr}");
            Console.WriteLine($"      Berörda versioner:  {vuln.Versions}");
            Console.WriteLine($"      Mer information:    {vuln.Url}");
        }
        Console.WriteLine($" -> Rekommendation: Uppgradera {pkg.Name} till en version som inte omfattas av de listade sårbarhetsintervallen.{Environment.NewLine}");
    }
    else
    {
        Console.WriteLine($"OK: Inga kända sårbarheter för {pkg.Name}.");
    }
}

#region DataModeller

// --- Datamodeller som krävs för att API-anropen ska fungera ---
record ServiceIndex(
    [property: JsonPropertyName("resources")]
    List<Resource> Resources
);

record Resource(
    [property: JsonPropertyName("@id")]
    string Id,
    [property: JsonPropertyName("@type")]
    string Type
);

record VulnerabilityFile(
    [property: JsonPropertyName("@name")]
    string Name,
    [property: JsonPropertyName("@id")]
    string Id
);

record Vulnerability(
    [property: JsonPropertyName("severity")]
    int Severity,
    [property: JsonPropertyName("versions")]
    string Versions,
    [property: JsonPropertyName("url")]
    string Url
);

record LockFile(
    [property: JsonPropertyName("dependencies")]
    Dictionary<string, Dictionary<string, LockDependency>> Dependencies
);

record LockDependency(
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("resolved")]
    string Resolved,
    [property: JsonPropertyName("contentHash")]
    string ContentHash
);

record NuGetPackage(string Name, string Version, string ContentHash);

#endregion
