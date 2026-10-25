using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ArvidsonFoto.Tests.E2E;

public sealed class E2EWebApplicationFactory : WebApplicationFactory<ArvidsonFoto.Program>
{
    public E2EWebApplicationFactory()
    {
        UseKestrel(0);
        using var client = CreateClient();
        BaseUrl = client.BaseAddress!.ToString().TrimEnd('/');
    }

    public string BaseUrl { get; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:UseInMemoryDatabase"] = "true",
                ["SmtpSettings:Server"] = ""
            }));
    }
}

[CollectionDefinition("E2E")]
public sealed class E2ECollectionDefinition : ICollectionFixture<E2EWebApplicationFactory>;

[Collection("E2E")]
public sealed class E2EHostTests(E2EWebApplicationFactory app)
{
    [Fact]
    public async Task HostedApp_ServesImagePurchasePage()
    {
        using var client = app.CreateClient();
        Assert.Equal("127.0.0.1", client.BaseAddress!.Host);
        var response = await client.GetAsync("/Info/Kop_av_bilder", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
