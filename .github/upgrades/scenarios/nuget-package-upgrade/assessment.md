# NuGet package upgrade assessment

_Mode: **quick assessment** — package API diffs only; no per-project source scan was run._

## Recommended versions

- **AngleSharp**: **1.8.2** (unified across 1 project(s)).
- **Aspire.Hosting.AppHost**: **13.5.4** (unified across 1 project(s)).
- **Aspire.Hosting.SqlServer**: **13.5.4** (unified across 1 project(s)).
- **Aspire.Hosting.Testing**: **13.5.4** (unified across 1 project(s)).
- **Azure.Core**: not referenced by any scoped project.
- **Azure.Identity**: **1.21.0** (unified across 1 project(s)).
- **BouncyCastle.Cryptography**: not referenced by any scoped project.
- **coverlet.collector**: **10.1.0** (unified across 3 project(s)).
- **Humanizer.Core**: not referenced by any scoped project.
- **JavaScriptEngineSwitcher.Extensions.MsDependencyInjection**: **3.31.0** (unified across 1 project(s)).
- **JavaScriptEngineSwitcher.V8**: **3.35.2** (unified across 1 project(s)).
- **KubernetesClient**: **19.0.2** (unified across 1 project(s)).
- **LigerShark.WebOptimizer.Core**: **3.0.477** (unified across 1 project(s)).
- **LigerShark.WebOptimizer.Sass**: **3.0.147** (unified across 1 project(s)).
- **MailKit**: **4.18.1** (unified across 1 project(s)).
- **MessagePack**: **3.1.10** (unified across 2 project(s)).
- **Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore**: **10.0.12** (unified across 1 project(s)).
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore**: **10.0.12** (unified across 1 project(s)).
- **Microsoft.AspNetCore.Identity.UI**: **10.0.12** (unified across 1 project(s)).
- **Microsoft.AspNetCore.Mvc.Testing**: **10.0.12** (unified across 2 project(s)).
- **Microsoft.AspNetCore.OpenApi**: **10.0.12** (unified across 1 project(s)).
- **Microsoft.Bcl.AsyncInterfaces**: not referenced by any scoped project.
- **Microsoft.Bcl.Cryptography**: not referenced by any scoped project.
- **Microsoft.Data.SqlClient**: not referenced by any scoped project.
- **Microsoft.EntityFrameworkCore.Design**: **10.0.12** (unified across 1 project(s)).
- **Microsoft.EntityFrameworkCore.InMemory**: **10.0.12** (unified across 1 project(s)).
- **Microsoft.EntityFrameworkCore.SqlServer**: **10.0.12** (unified across 1 project(s)).
- **Microsoft.EntityFrameworkCore.Tools**: **10.0.12** (unified across 1 project(s)).
- **Microsoft.Extensions.Caching.Memory**: not referenced by any scoped project.
- **Microsoft.Extensions.Configuration**: not referenced by any scoped project.
- **Microsoft.Extensions.Configuration.Abstractions**: not referenced by any scoped project.
- **Microsoft.Extensions.Configuration.Binder**: not referenced by any scoped project.
- **Microsoft.Extensions.Configuration.FileExtensions**: not referenced by any scoped project.
- **Microsoft.Extensions.Configuration.Json**: not referenced by any scoped project.
- **Microsoft.Extensions.DependencyInjection**: not referenced by any scoped project.
- **Microsoft.Extensions.DependencyModel**: not referenced by any scoped project.
- **Microsoft.Extensions.Diagnostics**: not referenced by any scoped project.
- **Microsoft.Extensions.Diagnostics.Abstractions**: not referenced by any scoped project.
- **Microsoft.Extensions.FileProviders.Abstractions**: not referenced by any scoped project.
- **Microsoft.Extensions.Hosting.Abstractions**: not referenced by any scoped project.
- **Microsoft.Extensions.Http**: not referenced by any scoped project.
- **Microsoft.Extensions.Http.Resilience**: **10.10.0** (unified across 1 project(s)).
- **Microsoft.Extensions.Logging**: not referenced by any scoped project.
- **Microsoft.Extensions.Logging.Configuration**: not referenced by any scoped project.
- **Microsoft.Extensions.ServiceDiscovery**: **10.10.0** (unified across 1 project(s)).
- **Microsoft.Identity.Client**: not referenced by any scoped project.
- **Microsoft.IdentityModel.JsonWebTokens**: not referenced by any scoped project.
- **Microsoft.NET.ILLink.Tasks**: **10.0.12** (unified across 1 project(s)).
- **Microsoft.NET.Test.Sdk**: **18.10.1** (unified across 3 project(s)).
- **Microsoft.Playwright.NUnit**: **1.63.0** (unified across 1 project(s)).
- **Microsoft.Testing.Extensions.Telemetry**: not referenced by any scoped project.
- **Microsoft.Testing.Extensions.TrxReport.Abstractions**: not referenced by any scoped project.
- **Microsoft.Testing.Platform**: not referenced by any scoped project.
- **Microsoft.Testing.Platform.MSBuild**: not referenced by any scoped project.
- **MimeKit**: **4.18.1** (unified across 1 project(s)).
- **MSTest.TestAdapter**: **4.4.1** (unified across 1 project(s)).
- **MSTest.TestFramework**: **4.4.1** (unified across 1 project(s)).
- **Newtonsoft.Json**: not referenced by any scoped project.
- **OpenTelemetry.Exporter.OpenTelemetryProtocol**: **1.19.1** (unified across 1 project(s)).
- **OpenTelemetry.Extensions.Hosting**: **1.19.1** (unified across 1 project(s)).
- **OpenTelemetry.Instrumentation.AspNetCore**: **1.19.0** (unified across 1 project(s)).
- **OpenTelemetry.Instrumentation.Http**: **1.19.0** (unified across 1 project(s)).
- **OpenTelemetry.Instrumentation.Runtime**: **1.19.0** (unified across 1 project(s)).
- **Polly.Core**: not referenced by any scoped project.
- **Polly.Extensions**: not referenced by any scoped project.
- **Scalar.AspNetCore**: **2.17.10** (unified across 1 project(s)).
- **Serilog**: **4.4.0** (unified across 1 project(s)).
- **Serilog.Settings.Configuration**: **10.0.1** (unified across 1 project(s)).
- **Serilog.Sinks.Console**: **6.1.1** (unified across 1 project(s)).
- **Serilog.Sinks.File**: **7.0.0** (unified across 1 project(s)).
- **System.IdentityModel.Tokens.Jwt**: not referenced by any scoped project.
- **System.Security.Cryptography.Pkcs**: not referenced by any scoped project.
- **System.Security.Cryptography.ProtectedData**: not referenced by any scoped project.
- **xunit**: not referenced by any scoped project.
- **xunit.runner.visualstudio**: **4.0.0** (unified across 2 project(s)).
- **xunit.v3**: **4.0.1** (unified across 2 project(s)).

## Public API changes

> **Types moved (namespace changed) are not removals.** A moved type keeps its name and members;
> the fix is a `using`-directive change, not a rewrite. Do not treat a moved type as deleted.

- **Microsoft.Extensions.Http.Resilience**: no source-breaking public API changes detected — see [`apidiff/Microsoft.Extensions.Http.Resilience.apidiff.md`](apidiff/Microsoft.Extensions.Http.Resilience.apidiff.md).
- **Microsoft.Extensions.ServiceDiscovery**: no source-breaking public API changes detected — see [`apidiff/Microsoft.Extensions.ServiceDiscovery.apidiff.md`](apidiff/Microsoft.Extensions.ServiceDiscovery.apidiff.md).
- **OpenTelemetry.Exporter.OpenTelemetryProtocol**: no source-breaking public API changes detected — see [`apidiff/OpenTelemetry.Exporter.OpenTelemetryProtocol.apidiff.md`](apidiff/OpenTelemetry.Exporter.OpenTelemetryProtocol.apidiff.md).
- **OpenTelemetry.Extensions.Hosting**: no source-breaking public API changes detected — see [`apidiff/OpenTelemetry.Extensions.Hosting.apidiff.md`](apidiff/OpenTelemetry.Extensions.Hosting.apidiff.md).
- **OpenTelemetry.Instrumentation.AspNetCore**: no source-breaking public API changes detected — see [`apidiff/OpenTelemetry.Instrumentation.AspNetCore.apidiff.md`](apidiff/OpenTelemetry.Instrumentation.AspNetCore.apidiff.md).
- **OpenTelemetry.Instrumentation.Http**: no source-breaking public API changes detected — see [`apidiff/OpenTelemetry.Instrumentation.Http.apidiff.md`](apidiff/OpenTelemetry.Instrumentation.Http.apidiff.md).
- **OpenTelemetry.Instrumentation.Runtime**: no source-breaking public API changes detected — see [`apidiff/OpenTelemetry.Instrumentation.Runtime.apidiff.md`](apidiff/OpenTelemetry.Instrumentation.Runtime.apidiff.md).
- **Microsoft.EntityFrameworkCore.Design**: 3 type(s) removed, 47 member(s) removed, 4 signature(s) changed — see [`apidiff/Microsoft.EntityFrameworkCore.Design.apidiff.md`](apidiff/Microsoft.EntityFrameworkCore.Design.apidiff.md).
- **MailKit**: no source-breaking public API changes detected — see [`apidiff/MailKit.apidiff.md`](apidiff/MailKit.apidiff.md).
- **MimeKit**: no source-breaking public API changes detected — see [`apidiff/MimeKit.apidiff.md`](apidiff/MimeKit.apidiff.md).
- **Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore**: no source-breaking public API changes detected — see [`apidiff/Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.apidiff.md`](apidiff/Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.apidiff.md).
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore**: no source-breaking public API changes detected — see [`apidiff/Microsoft.AspNetCore.Identity.EntityFrameworkCore.apidiff.md`](apidiff/Microsoft.AspNetCore.Identity.EntityFrameworkCore.apidiff.md).
- **Microsoft.AspNetCore.Identity.UI**: no source-breaking public API changes detected — see [`apidiff/Microsoft.AspNetCore.Identity.UI.apidiff.md`](apidiff/Microsoft.AspNetCore.Identity.UI.apidiff.md).
- **Microsoft.EntityFrameworkCore.SqlServer**: 20 type(s) removed, 66 member(s) removed — see [`apidiff/Microsoft.EntityFrameworkCore.SqlServer.apidiff.md`](apidiff/Microsoft.EntityFrameworkCore.SqlServer.apidiff.md).
- **Microsoft.EntityFrameworkCore.InMemory**: 1 type(s) removed — see [`apidiff/Microsoft.EntityFrameworkCore.InMemory.apidiff.md`](apidiff/Microsoft.EntityFrameworkCore.InMemory.apidiff.md).
- **Microsoft.EntityFrameworkCore.Tools**: no source-breaking public API changes detected — see [`apidiff/Microsoft.EntityFrameworkCore.Tools.apidiff.md`](apidiff/Microsoft.EntityFrameworkCore.Tools.apidiff.md).
- **Serilog**: no source-breaking public API changes detected — see [`apidiff/Serilog.apidiff.md`](apidiff/Serilog.apidiff.md).
- **JavaScriptEngineSwitcher.V8**: no source-breaking public API changes detected — see [`apidiff/JavaScriptEngineSwitcher.V8.apidiff.md`](apidiff/JavaScriptEngineSwitcher.V8.apidiff.md).
- **Microsoft.AspNetCore.OpenApi**: 1 type(s) removed, 1 member(s) removed — see [`apidiff/Microsoft.AspNetCore.OpenApi.apidiff.md`](apidiff/Microsoft.AspNetCore.OpenApi.apidiff.md).
- **Scalar.AspNetCore**: no source-breaking public API changes detected — see [`apidiff/Scalar.AspNetCore.apidiff.md`](apidiff/Scalar.AspNetCore.apidiff.md).
- **MessagePack**: no source-breaking public API changes detected — see [`apidiff/MessagePack.apidiff.md`](apidiff/MessagePack.apidiff.md).
- **coverlet.collector**: no source-breaking public API changes detected — see [`apidiff/coverlet.collector.apidiff.md`](apidiff/coverlet.collector.apidiff.md).
- **Microsoft.NET.Test.Sdk**: no source-breaking public API changes detected — see [`apidiff/Microsoft.NET.Test.Sdk.apidiff.md`](apidiff/Microsoft.NET.Test.Sdk.apidiff.md).
- **xunit.v3**: no source-breaking public API changes detected — see [`apidiff/xunit.v3.apidiff.md`](apidiff/xunit.v3.apidiff.md).
- **Microsoft.AspNetCore.Mvc.Testing**: no source-breaking public API changes detected — see [`apidiff/Microsoft.AspNetCore.Mvc.Testing.apidiff.md`](apidiff/Microsoft.AspNetCore.Mvc.Testing.apidiff.md).
- **MSTest.TestAdapter**: no source-breaking public API changes detected — see [`apidiff/MSTest.TestAdapter.apidiff.md`](apidiff/MSTest.TestAdapter.apidiff.md).
- **MSTest.TestFramework**: no source-breaking public API changes detected — see [`apidiff/MSTest.TestFramework.apidiff.md`](apidiff/MSTest.TestFramework.apidiff.md).
- **AngleSharp**: no source-breaking public API changes detected — see [`apidiff/AngleSharp.apidiff.md`](apidiff/AngleSharp.apidiff.md).
- **Microsoft.Playwright.NUnit**: no source-breaking public API changes detected — see [`apidiff/Microsoft.Playwright.NUnit.apidiff.md`](apidiff/Microsoft.Playwright.NUnit.apidiff.md).
- **Microsoft.NET.ILLink.Tasks**: no source-breaking public API changes detected — see [`apidiff/Microsoft.NET.ILLink.Tasks.apidiff.md`](apidiff/Microsoft.NET.ILLink.Tasks.apidiff.md).

## Breaking-change findings

- Version divergence findings (Pkg.0003): 0
- Requested-version-unsupported findings (Pkg.0002): 0

- Quick mode does not scan source, so there are no per-line `PkgApi` usage findings. Review the
  per-package API diffs above and rely on build errors during execution to pinpoint affected code.
- A full code scan can locate the exact source location of every breaking-change usage across the repo.
  It is opt-in and slower — re-run the assessment with `fullScan=true` only if the user requests it.

## Next steps

1. Proceed to planning to triage the API changes above and plan the code fixes (if any).

