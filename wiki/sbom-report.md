# 🛡️ Security & SBOM Scan Report
*Genererad den: 2026-10-18 16:32:25*

## 📊 Översikt
<table width="100%">
  <tr>
    <td align="center" width="25%" style="background-color:#f6f8fa; color:#1f2328; padding:15px; border-radius:6px; border:1px solid #d0d7de;"><b>Totalt skannade</b><br><font size="5" color="#1f2328">206</font></td>
    <td align="center" width="25%" style="background-color:#ffebe9; color:#1f2328; padding:15px; border-radius:6px; border:1px solid #ffc1c0;"><b>⚠️ Aktiva sårbarheter</b><br><font size="5" color="#cf222e"><b>2</b></font></td>
    <td align="center" width="25%" style="background-color:#fff8c5; color:#1f2328; padding:15px; border-radius:6px; border:1px solid #eee0b0;"><b>🔄 Versionskonflikter</b><br><font size="5" color="#9a6700"><b>1</b></font></td>
    <td align="center" width="25%" style="background-color:#ddf4ff; color:#1f2328; padding:15px; border-radius:6px; border:1px solid #b6e3ff;"><b>📦 Outdaterade paket</b><br><font size="5" color="#0969da"><b>79</b></font></td>
  </tr>
</table>

## 🚨 Aktiva sårbarheter funna
<table width="100%">
  <thead>
    <tr>
      <th align="left">Paketnamn</th>
      <th align="left">Installerad</th>
      <th align="left">Senaste</th>
      <th align="left">Allvarlighetsgrad & Detaljer</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><b>MessagePack</b></td>
      <td><code style="color:#cf222e; background-color:#ffebe9; padding:2px 4px; border-radius:4px; border:none;">3.1.8</code></td>
      <td><code style="color:#1f883d; background-color:#dafbe1; padding:2px 4px; border-radius:4px; border:none;">Okänd</code></td>
      <td>
        <div style="margin-bottom:6px;">
          <span style="background-color:#9a6700; color:white; padding:2px 6px; border-radius:10px; font-size:11px; font-weight:bold;">Medel</span>
          <span style="font-size:12px;"> Berör intervall: <code>[3.0.0, 3.1.9)</code></span> - <a href="https://github.com/advisories/GHSA-qhrr-8q5h-9q3h" target="_blank">Visa Advisory</a>
        </div>
      </td>
    </tr>
    <tr>
      <td><b>System.Security.Cryptography.Pkcs</b></td>
      <td><code style="color:#cf222e; background-color:#ffebe9; padding:2px 4px; border-radius:4px; border:none;">11.0.0-rc.2.26504.105</code></td>
      <td><code style="color:#1f883d; background-color:#dafbe1; padding:2px 4px; border-radius:4px; border:none;">Okänd</code></td>
      <td>
        <div style="margin-bottom:6px;">
          <span style="background-color:#bc4c00; color:white; padding:2px 6px; border-radius:10px; font-size:11px; font-weight:bold;">Hög</span>
          <span style="font-size:12px;"> Berör intervall: <code>[11.0.0-preview.1, 11.0.0-rc.1]</code></span> - <a href="https://github.com/advisories/GHSA-846f-f764-76mx" target="_blank">Visa Advisory</a>
        </div>
        <div style="margin-bottom:6px;">
          <span style="background-color:#9a6700; color:white; padding:2px 6px; border-radius:10px; font-size:11px; font-weight:bold;">Medel</span>
          <span style="font-size:12px;"> Berör intervall: <code>[11.0.0-preview.1, 11.0.0-rc.1]</code></span> - <a href="https://github.com/advisories/GHSA-2fmv-rv2c-59x7" target="_blank">Visa Advisory</a>
        </div>
      </td>
    </tr>
  </tbody>
</table>

## 🔄 Versionskonflikter upptäckta
<blockquote>💡 <b>Tips:</b> Standardisera dessa paket till en gemensam version i din <code>Directory.Packages.props</code> för att undvika oväntat runtime-beteende.</blockquote>
<table width="100%">
  <thead>
    <tr>
      <th align="left" width="40%">Paketnamn</th>
      <th align="left" width="60%">Installerade versioner</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><b>Microsoft.NET.StringTools</b></td>
      <td><code>18.4.0</code> &nbsp;|&nbsp; <code>18.9.6</code></td>
    </tr>
  </tbody>
</table>

## 📦 Komplett Paketförteckning (SBOM)
<table width="100%">
  <thead>
    <tr>
      <th align="left">Status</th>
      <th align="left">Paketnamn</th>
      <th align="left">Version</th>
      <th align="left">Senaste version</th>
      <th align="left">Hanteras i CPM?</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>AdvancedStringBuilder</b></td>
      <td><code>0.2.0</code></td>
      <td><code>0.2.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>JavaScriptEngineSwitcher.Extensions.MsDependencyInjection, JavaScriptEngineSwitcher.V8, LigerShark.WebOptimizer.Sass</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>AngleSharp</b></td>
      <td><code>1.5.1</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (1.5.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Aspire.Hosting</b></td>
      <td><code>13.6.1</code></td>
      <td><code>13.6.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Aspire.Hosting.AppHost</b></td>
      <td><code>13.6.1</code></td>
      <td><code>13.6.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>[Direkt SDK-beroende]</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Aspire.Hosting.SqlServer</b></td>
      <td><code>13.6.1</code></td>
      <td><code>13.6.1</code></td>
      <td><font size="2">Ja (13.6.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Aspire.Hosting.Testing</b></td>
      <td><code>13.6.1</code></td>
      <td><code>13.6.1</code></td>
      <td><font size="2">Ja (13.6.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>AspNetCore.HealthChecks.SqlServer</b></td>
      <td><code>9.0.0</code></td>
      <td><code>9.0.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>AspNetCore.HealthChecks.Uris</b></td>
      <td><code>9.0.0</code></td>
      <td><code>9.0.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Azure.Core</b></td>
      <td><code>1.62.0</code></td>
      <td><code>1.63.0</code></td>
      <td><font size="2">Ja (1.62.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Azure.Identity</b></td>
      <td><code>1.21.0</code></td>
      <td><code>1.21.0</code></td>
      <td><font size="2">Ja (1.21.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>BouncyCastle.Cryptography</b></td>
      <td><code>2.7.0</code></td>
      <td><code>2.7.0</code></td>
      <td><font size="2">Ja (2.7.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>coverlet.collector</b></td>
      <td><code>10.0.1</code></td>
      <td><code>10.1.0</code></td>
      <td><font size="2">Ja (10.0.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>DartSassHost</b></td>
      <td><code>2.0.0</code></td>
      <td><code>2.0.14</code></td>
      <td><font size="2">Nej (Transitivt)<br>LigerShark.WebOptimizer.Sass</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Fractions</b></td>
      <td><code>7.3.0</code></td>
      <td><code>8.3.2</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, KubernetesClient</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Google.Protobuf</b></td>
      <td><code>3.36.1</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Grpc.AspNetCore</b></td>
      <td><code>2.83.0</code></td>
      <td><code>2.84.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Grpc.AspNetCore.Server</b></td>
      <td><code>2.83.0</code></td>
      <td><code>2.84.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Grpc.AspNetCore.Server.ClientFactory</b></td>
      <td><code>2.83.0</code></td>
      <td><code>2.84.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Grpc.Core.Api</b></td>
      <td><code>2.83.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Grpc.Net.Client</b></td>
      <td><code>2.83.0</code></td>
      <td><code>2.84.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Grpc.Net.ClientFactory</b></td>
      <td><code>2.83.0</code></td>
      <td><code>2.84.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Grpc.Net.Common</b></td>
      <td><code>2.83.0</code></td>
      <td><code>2.84.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Grpc.Tools</b></td>
      <td><code>2.83.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Hex1b</b></td>
      <td><code>0.168.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Humanizer.Core</b></td>
      <td><code>3.0.10</code></td>
      <td><code>3.0.10</code></td>
      <td><font size="2">Ja (3.0.10)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>JavaScriptEngineSwitcher.Core</b></td>
      <td><code>3.31.0</code></td>
      <td><code>3.31.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>JavaScriptEngineSwitcher.Extensions.MsDependencyInjection, JavaScriptEngineSwitcher.V8, LigerShark.WebOptimizer.Sass</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>JavaScriptEngineSwitcher.Extensions.MsDependencyInjection</b></td>
      <td><code>3.31.0</code></td>
      <td><code>3.31.0</code></td>
      <td><font size="2">Ja (3.31.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>JavaScriptEngineSwitcher.V8</b></td>
      <td><code>3.35.2</code></td>
      <td><code>3.35.2</code></td>
      <td><font size="2">Ja (3.35.2)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>KubernetesClient</b></td>
      <td><code>19.0.2</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (19.0.2)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>LigerShark.WebOptimizer.Core</b></td>
      <td><code>3.0.477</code></td>
      <td><code>3.0.477</code></td>
      <td><font size="2">Ja (3.0.477)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>LigerShark.WebOptimizer.Sass</b></td>
      <td><code>3.0.147</code></td>
      <td><code>3.0.147</code></td>
      <td><font size="2">Ja (3.0.147)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>MailKit</b></td>
      <td><code>4.18.1</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (4.18.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>MessagePack.Annotations</b></td>
      <td><code>3.1.8</code></td>
      <td><code>3.1.12</code></td>
      <td><font size="2">Nej (Transitivt)<br>MessagePack</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>MessagePackAnalyzer</b></td>
      <td><code>3.1.8</code></td>
      <td><code>3.1.12</code></td>
      <td><font size="2">Nej (Transitivt)<br>MessagePack</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.ApplicationInsights</b></td>
      <td><code>2.23.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>xunit.v3, MSTest.TestAdapter</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.AspNetCore.Authorization</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.AspNetCore.Cryptography.Internal</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.AspNetCore.Cryptography.KeyDerivation</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.AspNetCore.Identity.EntityFrameworkCore</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.AspNetCore.Identity.UI</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.AspNetCore.Metadata</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.AspNetCore.Mvc.Testing</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.AspNetCore.OpenApi</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.AspNetCore.TestHost</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.AspNetCore.Mvc.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.AspNetCore.WebUtilities</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Bcl.AsyncInterfaces</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>10.0.13</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Bcl.Cryptography</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>10.0.13</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Build.Framework</b></td>
      <td><code>18.9.6</code></td>
      <td><code>18.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.ClearScript.Core</b></td>
      <td><code>7.5.1.1</code></td>
      <td><code>7.5.1.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>JavaScriptEngineSwitcher.V8</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.ClearScript.V8</b></td>
      <td><code>7.5.1.1</code></td>
      <td><code>7.5.1.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>JavaScriptEngineSwitcher.V8</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.ClearScript.V8.ICUData</b></td>
      <td><code>7.5.1.1</code></td>
      <td><code>7.5.1.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>JavaScriptEngineSwitcher.V8</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.ClearScript.V8.Native.win-arm64</b></td>
      <td><code>7.5.1.1</code></td>
      <td><code>7.5.1.1</code></td>
      <td><font size="2">Ja (7.5.1.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.CodeAnalysis.Analyzers</b></td>
      <td><code>5.9.0-1.26328.17</code></td>
      <td><code>5.9.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.CodeAnalysis.Common</b></td>
      <td><code>5.9.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.CodeAnalysis.CSharp</b></td>
      <td><code>5.9.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.CodeAnalysis.CSharp.Workspaces</b></td>
      <td><code>5.9.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.CodeAnalysis.Workspaces.Common</b></td>
      <td><code>5.9.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.CodeAnalysis.Workspaces.MSBuild</b></td>
      <td><code>5.9.0</code></td>
      <td><code>5.9.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.CodeCoverage</b></td>
      <td><code>18.9.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.NET.Test.Sdk</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Data.SqlClient</b></td>
      <td><code>7.1.1</code></td>
      <td><code>7.1.1</code></td>
      <td><font size="2">Ja (7.1.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Data.SqlClient.Extensions.Abstractions</b></td>
      <td><code>7.1.1</code></td>
      <td><code>7.1.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.SqlServer, Aspire.Hosting.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Data.SqlClient.Internal.Logging</b></td>
      <td><code>7.1.1</code></td>
      <td><code>7.1.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.SqlServer, Aspire.Hosting.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Data.SqlClient.SNI.runtime</b></td>
      <td><code>7.1.0</code></td>
      <td><code>7.1.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.SqlServer, Aspire.Hosting.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.EntityFrameworkCore</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.InMemory, Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Design, Microsoft.EntityFrameworkCore.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.EntityFrameworkCore.Abstractions</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.InMemory, Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Design, Microsoft.EntityFrameworkCore.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.EntityFrameworkCore.Analyzers</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.InMemory, Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Design, Microsoft.EntityFrameworkCore.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.EntityFrameworkCore.Design</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.EntityFrameworkCore.InMemory</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.EntityFrameworkCore.Relational</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Design, Microsoft.EntityFrameworkCore.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.EntityFrameworkCore.SqlServer</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.EntityFrameworkCore.Tools</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.AI.Abstractions</b></td>
      <td><code>10.5.2</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.AmbientMetadata.Application</b></td>
      <td><code>10.10.0</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Microsoft.Extensions.Http.Resilience, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Caching.Memory</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.Compliance.Abstractions</b></td>
      <td><code>10.10.0</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Microsoft.Extensions.Http.Resilience, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Configuration</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Configuration.Binder</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Configuration.CommandLine</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Configuration.EnvironmentVariables</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Configuration.FileExtensions</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Configuration.Json</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Configuration.UserSecrets</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.DependencyInjection</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.DependencyInjection.AutoActivation</b></td>
      <td><code>10.10.0</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Microsoft.Extensions.Http.Resilience, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.DependencyModel</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.Diagnostics</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>10.0.13</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.Diagnostics.ExceptionSummarization</b></td>
      <td><code>10.10.0</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Microsoft.Extensions.Http.Resilience, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Diagnostics.HealthChecks</b></td>
      <td><code>10.0.12</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Diagnostics.HealthChecks.Abstractions</b></td>
      <td><code>10.0.12</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Features</b></td>
      <td><code>10.0.12</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.FileProviders.Abstractions</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.FileProviders.Embedded</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.AspNetCore.Identity.UI</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.FileProviders.Physical</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.AspNetCore.OpenApi, Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Microsoft.AspNetCore.Mvc.Testing, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.FileSystemGlobbing</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.AspNetCore.OpenApi, Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Microsoft.AspNetCore.Mvc.Testing, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Hosting</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Http</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.Http.Diagnostics</b></td>
      <td><code>10.10.0</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Microsoft.Extensions.Http.Resilience, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.Http.Resilience</b></td>
      <td><code>10.10.0</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Ja (10.10.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Identity.Core</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Identity.Stores</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Logging</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Logging.Configuration</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Logging.Console</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Logging.Debug</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Logging.EventLog</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Logging.EventSource</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.ObjectPool</b></td>
      <td><code>10.0.12</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Extensions.Options.ConfigurationExtensions</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.AspNetCore.OpenApi, Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Microsoft.AspNetCore.Mvc.Testing, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.Resilience</b></td>
      <td><code>10.10.0</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Microsoft.Extensions.Http.Resilience, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.ServiceDiscovery</b></td>
      <td><code>10.10.0</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Ja (10.10.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.ServiceDiscovery.Abstractions</b></td>
      <td><code>10.10.0</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Microsoft.Extensions.ServiceDiscovery</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.Telemetry</b></td>
      <td><code>10.10.0</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Microsoft.Extensions.Http.Resilience, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Extensions.Telemetry.Abstractions</b></td>
      <td><code>10.10.0</code></td>
      <td><code>10.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Microsoft.Extensions.Http.Resilience, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Identity.Client</b></td>
      <td><code>4.88.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (4.88.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Identity.Client.Extensions.Msal</b></td>
      <td><code>4.84.2</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Azure.Identity</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.IdentityModel.Abstractions</b></td>
      <td><code>8.22.0</code></td>
      <td><code>8.23.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.SqlServer, Azure.Identity, Aspire.Hosting.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.IdentityModel.JsonWebTokens</b></td>
      <td><code>8.22.0</code></td>
      <td><code>8.23.0</code></td>
      <td><font size="2">Ja (8.22.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.IdentityModel.Logging</b></td>
      <td><code>8.22.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.SqlServer, Aspire.Hosting.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.IdentityModel.Protocols</b></td>
      <td><code>8.16.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.SqlServer, Aspire.Hosting.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.IdentityModel.Protocols.OpenIdConnect</b></td>
      <td><code>8.16.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.SqlServer, Aspire.Hosting.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.IdentityModel.Tokens</b></td>
      <td><code>8.22.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.SqlServer, Aspire.Hosting.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Net.Http.Headers</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#fff8c5; color:#744210; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">⚠️ Konflikt</span></td>
      <td><b>Microsoft.NET.StringTools</b></td>
      <td><code>18.4.0</code></td>
      <td><code>18.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>MessagePack, Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#fff8c5; color:#744210; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">⚠️ Konflikt</span></td>
      <td><b>Microsoft.NET.StringTools</b></td>
      <td><code>18.9.6</code></td>
      <td><code>18.11.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.NET.Test.Sdk</b></td>
      <td><code>18.9.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (18.9.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.OpenApi</b></td>
      <td><code>3.10.2</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.AspNetCore.OpenApi</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Playwright</b></td>
      <td><code>1.62.0</code></td>
      <td><code>1.63.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.Playwright.NUnit</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Playwright.NUnit</b></td>
      <td><code>1.62.0</code></td>
      <td><code>1.63.0</code></td>
      <td><font size="2">Ja (1.62.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Playwright.TestAdapter</b></td>
      <td><code>1.62.0</code></td>
      <td><code>1.63.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.Playwright.NUnit</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.SqlServer.Server</b></td>
      <td><code>1.0.0</code></td>
      <td><code>1.0.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.SqlServer, Aspire.Hosting.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Testing.Extensions.Telemetry</b></td>
      <td><code>2.3.3</code></td>
      <td><code>2.5.1</code></td>
      <td><font size="2">Ja (2.3.3)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Testing.Extensions.TrxReport.Abstractions</b></td>
      <td><code>2.3.3</code></td>
      <td><code>2.5.1</code></td>
      <td><font size="2">Ja (2.3.3)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Testing.Extensions.VSTestBridge</b></td>
      <td><code>2.3.3</code></td>
      <td><code>2.5.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>MSTest.TestAdapter</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Testing.Platform</b></td>
      <td><code>2.3.3</code></td>
      <td><code>2.5.1</code></td>
      <td><font size="2">Ja (2.3.3)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Testing.Platform.MSBuild</b></td>
      <td><code>2.3.3</code></td>
      <td><code>2.5.1</code></td>
      <td><font size="2">Ja (2.3.3)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.TestPlatform.ObjectModel</b></td>
      <td><code>18.9.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.Playwright.NUnit, Microsoft.NET.Test.Sdk, MSTest.TestAdapter</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.TestPlatform.TestHost</b></td>
      <td><code>18.9.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.NET.Test.Sdk</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.VisualStudio.SolutionPersistence</b></td>
      <td><code>1.0.52</code></td>
      <td><code>1.0.52</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.VisualStudio.Threading.Only</b></td>
      <td><code>17.14.15</code></td>
      <td><code>18.12.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.VisualStudio.Validation</b></td>
      <td><code>17.13.22</code></td>
      <td><code>17.13.22</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Microsoft.Win32.Registry</b></td>
      <td><code>5.0.0</code></td>
      <td><code>5.0.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>xunit.v3</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Microsoft.Win32.SystemEvents</b></td>
      <td><code>6.0.0</code></td>
      <td><code>10.0.13</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>MimeKit</b></td>
      <td><code>4.18.1</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (4.18.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>ModelContextProtocol</b></td>
      <td><code>1.4.1</code></td>
      <td><code>2.2.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>ModelContextProtocol.Core</b></td>
      <td><code>1.4.1</code></td>
      <td><code>2.2.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Mono.TextTemplating</b></td>
      <td><code>3.0.0</code></td>
      <td><code>3.0.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>MSTest.Analyzers</b></td>
      <td><code>4.3.3</code></td>
      <td><code>4.5.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>MSTest.TestFramework</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>MSTest.TestAdapter</b></td>
      <td><code>4.3.3</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (4.3.3)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>MSTest.TestFramework</b></td>
      <td><code>4.3.3</code></td>
      <td><code>4.5.1</code></td>
      <td><font size="2">Ja (4.3.3)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Nerdbank.MessagePack</b></td>
      <td><code>1.2.4</code></td>
      <td><code>1.4.82</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Nerdbank.Streams</b></td>
      <td><code>2.13.16</code></td>
      <td><code>2.14.405</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Newtonsoft.Json</b></td>
      <td><code>13.0.4</code></td>
      <td><code>13.0.4</code></td>
      <td><font size="2">Ja (13.0.4)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>NUglify</b></td>
      <td><code>1.21.15</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>LigerShark.WebOptimizer.Core</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>NUnit</b></td>
      <td><code>3.13.2</code></td>
      <td><code>5.0.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.Playwright.NUnit</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>NUnit3TestAdapter</b></td>
      <td><code>4.0.0</code></td>
      <td><code>6.3.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.Playwright.NUnit</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>OpenTelemetry</b></td>
      <td><code>1.19.1</code></td>
      <td><code>1.20.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, OpenTelemetry.Exporter.OpenTelemetryProtocol, OpenTelemetry.Extensions.Hosting, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>OpenTelemetry.Api</b></td>
      <td><code>1.19.1</code></td>
      <td><code>1.20.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, OpenTelemetry.Instrumentation.Runtime, OpenTelemetry.Instrumentation.AspNetCore, OpenTelemetry.Instrumentation.Http, OpenTelemetry.Exporter.OpenTelemetryProtocol, OpenTelemetry.Extensions.Hosting, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>OpenTelemetry.Api.ProviderBuilderExtensions</b></td>
      <td><code>1.19.1</code></td>
      <td><code>1.20.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, OpenTelemetry.Instrumentation.AspNetCore, OpenTelemetry.Instrumentation.Http, OpenTelemetry.Exporter.OpenTelemetryProtocol, OpenTelemetry.Extensions.Hosting, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>OpenTelemetry.Exporter.OpenTelemetryProtocol</b></td>
      <td><code>1.19.1</code></td>
      <td><code>1.20.0</code></td>
      <td><font size="2">Ja (1.19.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>OpenTelemetry.Extensions.Hosting</b></td>
      <td><code>1.19.1</code></td>
      <td><code>1.20.0</code></td>
      <td><font size="2">Ja (1.19.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>OpenTelemetry.Instrumentation.AspNetCore</b></td>
      <td><code>1.19.0</code></td>
      <td><code>1.20.0</code></td>
      <td><font size="2">Ja (1.19.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>OpenTelemetry.Instrumentation.Http</b></td>
      <td><code>1.19.0</code></td>
      <td><code>1.20.0</code></td>
      <td><font size="2">Ja (1.19.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>OpenTelemetry.Instrumentation.Runtime</b></td>
      <td><code>1.19.0</code></td>
      <td><code>1.20.0</code></td>
      <td><font size="2">Ja (1.19.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Polly.Core</b></td>
      <td><code>8.7.0</code></td>
      <td><code>8.8.0</code></td>
      <td><font size="2">Ja (8.7.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Polly.Extensions</b></td>
      <td><code>8.7.0</code></td>
      <td><code>8.8.0</code></td>
      <td><font size="2">Ja (8.7.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>Polly.RateLimiting</b></td>
      <td><code>8.4.2</code></td>
      <td><code>8.8.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Okänd källa, Microsoft.Extensions.Http.Resilience, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>PolyType</b></td>
      <td><code>1.3.1</code></td>
      <td><code>1.4.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>QRCoder</b></td>
      <td><code>1.7.0</code></td>
      <td><code>1.8.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Scalar.AspNetCore</b></td>
      <td><code>2.17.10</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (2.17.10)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Semver</b></td>
      <td><code>3.0.0</code></td>
      <td><code>3.0.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Serilog</b></td>
      <td><code>4.4.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (4.4.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Serilog.Settings.Configuration</b></td>
      <td><code>10.0.1</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (10.0.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Serilog.Sinks.Console</b></td>
      <td><code>6.1.1</code></td>
      <td><code>6.1.1</code></td>
      <td><font size="2">Ja (6.1.1)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>Serilog.Sinks.File</b></td>
      <td><code>7.0.0</code></td>
      <td><code>7.0.0</code></td>
      <td><font size="2">Ja (7.0.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>StreamJsonRpc</b></td>
      <td><code>2.25.29</code></td>
      <td><code>2.22.23</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>System.ClientModel</b></td>
      <td><code>1.15.0</code></td>
      <td><code>1.16.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>Azure.Identity</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.CodeDom</b></td>
      <td><code>6.0.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.Composition</b></td>
      <td><code>10.0.1</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.Composition.AttributedModel</b></td>
      <td><code>10.0.1</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.Composition.Convention</b></td>
      <td><code>10.0.1</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.Composition.Hosting</b></td>
      <td><code>10.0.1</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.Composition.Runtime</b></td>
      <td><code>10.0.1</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.Composition.TypedParts</b></td>
      <td><code>10.0.1</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.Design</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.Configuration.ConfigurationManager</b></td>
      <td><code>9.0.18</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Microsoft.EntityFrameworkCore.SqlServer, Aspire.Hosting.SqlServer</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.Diagnostics.EventLog</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.Drawing.Common</b></td>
      <td><code>6.0.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.IdentityModel.Tokens.Jwt</b></td>
      <td><code>8.22.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (8.22.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>System.IO.Hashing</b></td>
      <td><code>10.0.12</code></td>
      <td><code>10.0.13</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>System.Memory.Data</b></td>
      <td><code>10.0.10</code></td>
      <td><code>10.0.13</code></td>
      <td><font size="2">Nej (Transitivt)<br>Azure.Identity</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.Security.AccessControl</b></td>
      <td><code>6.0.1</code></td>
      <td><code>6.0.1</code></td>
      <td><font size="2">Nej (Transitivt)<br>xunit.v3</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>System.Security.Cryptography.ProtectedData</b></td>
      <td><code>11.0.0-rc.2.26504.105</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Ja (11.0.0-rc.2.26504.105)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>System.Threading.RateLimiting</b></td>
      <td><code>9.0.18</code></td>
      <td><code>10.0.13</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.SqlServer, Aspire.Hosting.Testing</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>xunit.analyzers</b></td>
      <td><code>2.0.0</code></td>
      <td><code>2.2.0</code></td>
      <td><font size="2">Nej (Transitivt)<br>xunit.v3</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>xunit.runner.visualstudio</b></td>
      <td><code>4.0.0</code></td>
      <td><code>4.0.1</code></td>
      <td><font size="2">Ja (4.0.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>xunit.v3</b></td>
      <td><code>4.0.0</code></td>
      <td><code>4.0.2</code></td>
      <td><font size="2">Ja (4.0.0)</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>xunit.v3.assert</b></td>
      <td><code>4.0.0</code></td>
      <td><code>4.0.2</code></td>
      <td><font size="2">Nej (Transitivt)<br>xunit.v3</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>xunit.v3.common</b></td>
      <td><code>4.0.0</code></td>
      <td><code>4.0.2</code></td>
      <td><font size="2">Nej (Transitivt)<br>xunit.v3</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>xunit.v3.core.mtp-v2</b></td>
      <td><code>4.0.0</code></td>
      <td><code>4.0.2</code></td>
      <td><font size="2">Nej (Transitivt)<br>xunit.v3</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>xunit.v3.extensibility.core</b></td>
      <td><code>4.0.0</code></td>
      <td><code>4.0.2</code></td>
      <td><font size="2">Nej (Transitivt)<br>xunit.v3</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>xunit.v3.mtp-v2</b></td>
      <td><code>4.0.0</code></td>
      <td><code>4.0.2</code></td>
      <td><font size="2">Nej (Transitivt)<br>xunit.v3</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>xunit.v3.runner.common</b></td>
      <td><code>4.0.0</code></td>
      <td><code>4.0.2</code></td>
      <td><font size="2">Nej (Transitivt)<br>xunit.v3</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#ddf4ff; color:#0969da; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">🔄 Uppdatering</span></td>
      <td><b>xunit.v3.runner.inproc.console</b></td>
      <td><code>4.0.0</code></td>
      <td><code>4.0.2</code></td>
      <td><font size="2">Nej (Transitivt)<br>xunit.v3</font></td>
    </tr>
    <tr>
      <td><span style="background-color:#dafbe1; color:#1f883d; padding:2px 6px; border-radius:4px; font-size:11px; font-weight:bold; display:inline-block;">✓ OK</span></td>
      <td><b>YamlDotNet</b></td>
      <td><code>18.1.0</code></td>
      <td><code>Okänd</code></td>
      <td><font size="2">Nej (Transitivt)<br>Aspire.Hosting.AppHost, Aspire.Hosting.SqlServer, Aspire.Hosting.Testing, KubernetesClient</font></td>
    </tr>
  </tbody>
</table>
