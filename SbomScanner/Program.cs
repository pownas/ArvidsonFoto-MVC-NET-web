using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

#pragma warning disable CA1303 // Do not pass literals as localized parameters
#pragma warning disable CA1308 // Normalize strings to uppercase

string jsonContent = await File.ReadAllTextAsync("packages.lock.json").ConfigureAwait(true);
var lockFile = JsonSerializer.Deserialize<LockFile>(jsonContent);

var extractedPackages = new List<NuGetPackage>();

foreach (var framework in lockFile!.Dependencies)
{
    foreach (var package in framework.Value)
    {
        if (package.Value.Type == "Project" || string.IsNullOrEmpty(package.Value.ContentHash))
            continue; // Skippa interna projekt

        extractedPackages.Add(new NuGetPackage(
            package.Key,
            package.Value.Resolved ?? "Okänd",
            package.Value.ContentHash));
    }
}

var doc = XDocument.Load("Directory.Packages.props");
var centralPackages = doc.Descendants("PackageVersion")
    .Select(node => new
    {
        Id = node.Attribute("Include")?.Value ?? node.Attribute("Update")?.Value ?? "",
        Version = node.Attribute("Version")?.Value ?? ""
    })
    .Where(p => !string.IsNullOrWhiteSpace(p.Id))
    .ToList();

using var client = new HttpClient();

Console.WriteLine("Hämtar Service Index från NuGet...");

// 1. Hämta NuGet V3 Service Index
var serviceIndexUrl = "https://api.nuget.org/v3/index.json";
var serviceIndex = await client.GetFromJsonAsync<ServiceIndex>(serviceIndexUrl).ConfigureAwait(true);

// 2. Hitta slutpunkten för VulnerabilityInfo i indexet
var vulnResourceUrl = serviceIndex?.Resources
    .FirstOrDefault(r => r.Type.StartsWith("VulnerabilityInfo", StringComparison.OrdinalIgnoreCase))?.Id;

if (vulnResourceUrl is null)
{
  Console.WriteLine("Kunde inte hitta sårbarhets-API:et.");
  return;
}

// 3. Hämta fillistan (innehåller referenser till base.json)
var vulnFiles = await client.GetFromJsonAsync<List<VulnerabilityFile>>(vulnResourceUrl).ConfigureAwait(true);

// HÄR definieras den saknade variabeln:
string? baseJsonUrl = vulnFiles?.FirstOrDefault(f => f.Name == "base.json")?.Id;

if (baseJsonUrl is null)
{
    Console.WriteLine("Kunde inte hitta base.json.");
    return;
}

Console.WriteLine($"Laddar ner sårbarhetsdatabasen från: {baseJsonUrl}");

// 4. Ladda ner sårbarhetsdatabasen (base.json)
var vulnerabilities = await client.GetFromJsonAsync<Dictionary<string, List<Vulnerability>>>(baseJsonUrl).ConfigureAwait(true);

// 5. Scanna dina extraherade paket i minnet
Console.WriteLine("\nPåbörjar skanning...");
foreach (var pkg in extractedPackages)
{
  string searchKey = pkg.Name.ToLowerInvariant(); // Viktigt med små bokstäver

  if (vulnerabilities!.TryGetValue(searchKey, out var vulns))
    {
        Console.WriteLine($"VARNING: Hittade {vulns.Count} kända sårbarheter i {pkg.Name}!");
        // Kolla om din version (pkg.Version) faller inom vulns[i].Versions
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
    string Versions
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
