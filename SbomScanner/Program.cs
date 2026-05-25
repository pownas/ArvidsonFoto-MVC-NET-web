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

// Gruppera paketen efter namn och sortera grupperna i bokstavsordning (A-Ö)
var groupedPackages = extractedPackages.GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase).OrderBy(g => g.Key);

foreach (var packageGroup in groupedPackages)
{
    string packageName = packageGroup.Key;
    // NuGet-API:et kräver att alla paket-ID:n är i gemener (små bokstäver) vid sökning
    string searchKey = packageName.ToLowerInvariant();

    // Kontrollera om paketet har mer än en unik version installerad
    bool hasVersionMismatch = packageGroup.Select(p => p.Version).Distinct().Count() > 1;

    // Om det finns en versionskonflikt, varna användaren högst upp för detta paket
    if (hasVersionMismatch)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⚠️ VARNING: Versionskonflikt upptäckt för {packageName}!");
        Console.Write("    Installerad i följande versioner: ");
        // Sorterar även versionsnumren i varningstexten
        Console.WriteLine(string.Join(", ", packageGroup.Select(p => p.Version).Distinct().OrderBy(v => v)));
        Console.WriteLine("    -> Tips: Lås paketet till en gemensam version i Directory.Packages.props för att fixa detta.");
        Console.ResetColor();
    }

    // Sorterar de enskilda paketen i gruppen efter version så att de också skrivs ut i ordning
    var sortedPackageItems = packageGroup.OrderBy(p => p.Version);

    foreach (var pkg in sortedPackageItems)
    {
        var cpmMatch = centralPackages.FirstOrDefault(c => c.Id.Equals(pkg.Name, StringComparison.OrdinalIgnoreCase));
        string cpmStatus = cpmMatch != null
            ? $"Ja (Definierad till: {cpmMatch.Version})"
            : "Nej (Transitivt beroende)";

        // Hämta live-version från NuGet
        string latestNuGetVersion = "Okänd";
        try
        {
            var packageRegUrl = $"{regResourceUrl.TrimEnd('/')}/{searchKey}/index.json";
            var regIndex = await client.GetFromJsonAsync<NugetRegistrationIndex>(packageRegUrl).ConfigureAwait(true);
            if (regIndex?.Pages != null && regIndex.Pages.Count > 0)
            {
                var lastPage = regIndex.Pages[^1];
                if (lastPage.Items != null && lastPage.Items.Count > 0)
                {
                    latestNuGetVersion = lastPage.Items[^1].CatalogEntry.Version;
                }
            }
        }
        catch { /* Ignorera API-missar */ }

        // Filtrera fram om sårbarheterna faktiskt drabbar DENNA specifika version
        var activeVulnerabilities = new List<Vulnerability>();
        if (vulnerabilities!.TryGetValue(searchKey, out var vulnList))
        {
            foreach (var v in vulnList)
            {
                if (IsVersionAffected(pkg.Version, v.Versions))
                {
                    activeVulnerabilities.Add(v);
                }
            }
        }

        // --- Kontroll BASERAT PÅ OM SÅRBARHETEN ÄR AKTIV ELLER PATCHAD ---
        if (activeVulnerabilities.Count > 0)
        {
            // Din version ÄR drabbad av en eller flera sårbarheter!
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[VARNING] Aktiv sårbarhet funnen i: {pkg.Name} (v. {pkg.Version})");
            Console.ResetColor();

            Console.WriteLine($"  -> Nuvarande installerad version:  {pkg.Version}");
            Console.WriteLine($"  -> Hanteras centralt i CPM?         {cpmStatus}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  -> Senaste live-version på NuGet:  {latestNuGetVersion}");
            Console.ResetColor();
            Console.WriteLine($"  -> Hittade {activeVulnerabilities.Count} aktiva sårbarhetsintervall för denna version:");

            foreach (var vuln in activeVulnerabilities)
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[OK] ");
            Console.ResetColor();
            Console.Write($"{pkg.Name} (Version: {pkg.Version}) | CPM: {cpmStatus} ");

            if (vulnList != null && vulnList.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("[Säkrad/Patchad] ");
                Console.ResetColor();
            }

            if (latestNuGetVersion != "Okänd" && latestNuGetVersion != pkg.Version)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"[Nyare version finns: {latestNuGetVersion}]");
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    // Lägg till en extra tomrad efter varje paketgrupp för att göra rapporten mer lättläst
    if (hasVersionMismatch)
    {
        Console.WriteLine();
    }
}

#region Hjälpmetoder

// --- LÄGG TILL DENNA HJÄLPMETOD LÄNGST NER I PROGRAM.CS (Utanför loopen eller i en helper-klass) ---
static bool IsVersionAffected(string currentVersionStr, string rangeStr)
{
    if (string.IsNullOrWhiteSpace(rangeStr)) return false;
    if (!Version.TryParse(currentVersionStr.Split('-')[0], out var currentVersion)) return false; // Skippa eventuell -beta/-rc vid ren sifferjämförelse

    rangeStr = rangeStr.Trim();

    // 1. Hantera standard NuGet-intervall notation t.ex. (, 13.0.1) eller [1.0.0, 2.0.0)
    if (rangeStr.StartsWith('(') || rangeStr.StartsWith('['))
    {
        var parts = rangeStr.Substring(1, rangeStr.Length - 2).Split(',');
        if (parts.Length != 2) return false;

        bool isMinInclusive = rangeStr.StartsWith('[');
        bool isMaxInclusive = rangeStr.EndsWith(']');

        string minStr = parts[0].Trim();
        string maxStr = parts[1].Trim();

        // Kolla minimum-gränsen
        if (!string.IsNullOrEmpty(minStr) && Version.TryParse(minStr.Split('-')[0], out var minVersion))
        {
            if (isMinInclusive && currentVersion < minVersion) return false;
            if (!isMinInclusive && currentVersion <= minVersion) return false;
        }

        // Kolla maximum-gränsen
        if (!string.IsNullOrEmpty(maxStr) && Version.TryParse(maxStr.Split('-')[0], out var maxVersion))
        {
            if (isMaxInclusive && currentVersion > maxVersion) return false;
            if (!isMaxInclusive && currentVersion >= maxVersion) return false;
        }

        return true;
    }

    // 2. Hantera enkla jämförelser om de skulle dyka upp (t.ex. "<= 13.0.1" eller "< 13.0.1")
    if (rangeStr.StartsWith("<="))
    {
        if (Version.TryParse(rangeStr.Replace("<=", "").Trim().Split('-')[0], out var v)) return currentVersion <= v;
    }
    if (rangeStr.StartsWith('<'))
    {
        if (Version.TryParse(rangeStr.Replace("<", "").Trim().Split('-')[0], out var v)) return currentVersion < v;
    }

    return false;
}

#endregion

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
