# 01.03-hosting-tests: Uppdatera hosting- och testpaket

## Objective
Uppdatera aktiva stabila paket för app-hosting och testprojekten utan att ändra befintliga RC1-paket eller inaktiva centrala poster.

## Scope
Centrala versioner i `Directory.Packages.props`: `MessagePack` 3.1.8→3.1.10, `AngleSharp` 1.5.1→1.8.2, `coverlet.collector` 10.0.1→10.1.0, `Microsoft.NET.Test.Sdk` 18.9.0→18.10.1, `Microsoft.Playwright.NUnit` 1.62.0→1.63.0, `MSTest.TestAdapter` och `MSTest.TestFramework` 4.3.3→4.4.1 samt `xunit.v3` 4.0.0→4.0.1. `MessagePack` används både av AppHost och Unit tests. Behåll `Microsoft.AspNetCore.Mvc.Testing` och övriga 11 RC1-paket. API-diffen rapporterade inga breaking changes för paketen som uppdateras.

## Steps
1. Ändra endast de åtta valda `PackageVersion`-posterna i root `Directory.Packages.props`.
2. Återställ och bygg AppHost och samtliga tre testprojekt.
3. Kör samtliga testprojekt och åtgärda fel samt alla build warnings.

**Done when**: De åtta versionerna är uppdaterade centralt, host/test-projekten bygger och alla testprojekt passerar utan kvarvarande build warnings.
