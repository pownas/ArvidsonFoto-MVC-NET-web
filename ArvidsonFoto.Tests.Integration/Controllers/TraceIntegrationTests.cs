using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using AngleSharp.Html.Parser;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace ArvidsonFoto.Tests.Integration.Controllers;

[TestClass]
public sealed class TraceIntegrationTests
{
    [TestMethod]
    public async Task PageAndLocalImage_ExportSameTraceWithoutIndependentImageServerSpan()
    {
        using var activityScope = new ActivityScope();
        using var processor = new RecordingProcessor();
        using var factory = new TraceFactory(processor, correlateImages: true).UseLocalKestrel();
        using var bootstrapClient = factory.CreateClient();
        var endpoint = bootstrapClient.BaseAddress!;

        var page = await GetRawAsync(endpoint, "/");
        Assert.IsTrue(page.StartsWith("HTTP/1.1 200", StringComparison.Ordinal), page);
        var document = new HtmlParser().ParseDocument(page);
        var imageUrl = document.QuerySelector("img[src*='af_traceparent']")?.GetAttribute("src")
            ?? throw new AssertFailedException("A local image must carry the page trace context.");
        var pageSpan = processor.Activities.Single(activity =>
            activity.Kind == ActivityKind.Server && Equals(activity.GetTagItem("url.path"), "/"));

        var response = await GetRawAsync(endpoint, new Uri(endpoint, imageUrl).PathAndQuery);
        Assert.IsTrue(response.StartsWith("HTTP/1.1 200", StringComparison.Ordinal), response[..Math.Min(300, response.Length)]);

        var imageSpans = processor.Activities.Where(activity =>
            activity.Kind == ActivityKind.Server && activity.GetTagItem("url.path")?.ToString() == "/img/Torbjorn2020.jpg").ToArray();
        Assert.AreEqual(1, imageSpans.Length, "Only the correlated image server span should be exported.");
        var imageSpan = imageSpans[0];
        Assert.AreEqual("ArvidsonFoto", imageSpan.Source.Name);
        Assert.AreEqual(pageSpan.TraceId, imageSpan.TraceId);
        Assert.AreEqual(pageSpan.SpanId, imageSpan.ParentSpanId);
        Assert.AreEqual("image.fetch /img/Torbjorn2020.jpg", imageSpan.DisplayName);
        Assert.AreEqual(200, imageSpan.GetTagItem("http.response.status_code"));
        Assert.AreEqual("image/jpeg", imageSpan.GetTagItem("http.response.header.content-type"));
        Assert.IsTrue((long)imageSpan.GetTagItem("http.response.body.size")! > 0);
        Assert.AreEqual(true, imageSpan.GetTagItem("arvidsonfoto.image.correlated"));
        Assert.IsTrue(processor.Activities.Any(activity =>
            activity.TraceId == pageSpan.TraceId && activity.DisplayName == "mvc.action Home.Index"));
        Assert.IsTrue(processor.Activities.Any(activity =>
            activity.TraceId == pageSpan.TraceId && activity.DisplayName == "mvc.view Index"));
        Assert.IsFalse(imageSpan.TagObjects.Any(tag => tag.Key.Contains("query", StringComparison.Ordinal)));
    }

    [TestMethod]
    public async Task DisabledCorrelation_LeavesImageUrlAndIndependentServerTraceUnchanged()
    {
        using var activityScope = new ActivityScope();
        using var processor = new RecordingProcessor();
        using var factory = new TraceFactory(processor, correlateImages: false).UseLocalKestrel();
        using var bootstrapClient = factory.CreateClient();
        var endpoint = bootstrapClient.BaseAddress!;
        var page = await GetRawAsync(endpoint, "/");
        Assert.IsTrue(page.StartsWith("HTTP/1.1 200", StringComparison.Ordinal), page);
        var document = new HtmlParser().ParseDocument(page);
        Assert.IsNull(document.QuerySelector("img[src*='af_traceparent']"));
        var pageSpan = processor.Activities.Single(activity =>
            activity.Kind == ActivityKind.Server && Equals(activity.GetTagItem("url.path"), "/"));

        var response = await GetRawAsync(endpoint,
            $"/img/Torbjorn2020.jpg?af_traceparent={pageSpan.Id}");
        Assert.IsTrue(response.StartsWith("HTTP/1.1 200", StringComparison.Ordinal));
        var imageSpan = processor.Activities.Single(activity =>
            activity.Kind == ActivityKind.Server && Equals(activity.GetTagItem("url.path"), "/img/Torbjorn2020.jpg"));
        Assert.AreNotEqual(pageSpan.TraceId, imageSpan.TraceId);
        Assert.AreEqual("GET /img/Torbjorn2020.jpg", imageSpan.DisplayName);
        Assert.IsFalse(processor.Activities.Any(activity => activity.DisplayName.StartsWith("image.fetch", StringComparison.Ordinal)));
    }

    [TestMethod]
    public async Task Dagrovfaglar_ApiRequestsSharePageTraceAndDescribeImageWork()
    {
        using var activityScope = new ActivityScope();
        using var processor = new RecordingProcessor();
        using var factory = new TraceFactory(processor, correlateImages: true).UseLocalKestrel();
        using var bootstrapClient = factory.CreateClient();
        var endpoint = bootstrapClient.BaseAddress!;
        const string path = "/bilder/faglar/dagrovfaglar";

        var page = await GetRawAsync(endpoint, path);
        Assert.IsTrue(page.StartsWith("HTTP/1.1 200", StringComparison.Ordinal), page[..Math.Min(300, page.Length)]);
        var parent = new HtmlParser().ParseDocument(page)
            .QuerySelector("meta[name='arvidsonfoto-traceparent']")?.GetAttribute("content")
            ?? throw new AssertFailedException("The page must expose its W3C request context.");
        var pageSpan = processor.Activities.Single(activity =>
            activity.Kind == ActivityKind.Server && Equals(activity.GetTagItem("url.path"), path));
        Assert.AreEqual(pageSpan.Id, parent);
        Assert.AreEqual(10, pageSpan.GetTagItem("arvidsonfoto.category.id"));
        StringAssert.Contains(pageSpan.DisplayName, "Dagrovfåglar");

        const string imagePath = "/api/image/getoneimagefromcategory/10";
        var tooltip = await GetRawAsync(endpoint, imagePath + "?culture=sv-SE",
            $"traceparent: {parent}\r\nX-ArvidsonFoto-Operation: tooltip.prefetch\r\n");
        Assert.IsTrue(tooltip.StartsWith("HTTP/1.1 200", StringComparison.Ordinal), tooltip[..Math.Min(300, tooltip.Length)]);
        const string pagingPath = "/api/bilder/faglar/dagrovfaglar";
        var paging = await GetRawAsync(endpoint, pagingPath + "?page=1",
            $"traceparent: {parent}\r\nX-ArvidsonFoto-Operation: gallery.next-page\r\n");
        Assert.IsTrue(paging.StartsWith("HTTP/1.1 200", StringComparison.Ordinal), paging[..Math.Min(300, paging.Length)]);

        var serverSpans = processor.Activities.Where(activity => activity.Kind == ActivityKind.Server).ToArray();
        Assert.AreEqual(3, serverSpans.Length);
        Assert.IsTrue(serverSpans.All(activity => activity.TraceId == pageSpan.TraceId));
        foreach (var api in serverSpans.Where(activity => activity != pageSpan))
        {
            Assert.AreEqual(pageSpan.SpanId, api.ParentSpanId);
            StringAssert.Contains(api.DisplayName, "category");
        }
        var tooltipSpan = serverSpans.Single(activity => Equals(activity.GetTagItem("url.path"), imagePath));
        Assert.AreEqual("tooltip.prefetch", tooltipSpan.GetTagItem("arvidsonfoto.browser.operation"));
        Assert.IsTrue((int)tooltipSpan.GetTagItem("arvidsonfoto.image.id")! > 0);
        var pagingSpan = serverSpans.Single(activity => Equals(activity.GetTagItem("url.path"), pagingPath));
        Assert.AreEqual("gallery.next-page", pagingSpan.GetTagItem("arvidsonfoto.browser.operation"));
        Assert.AreEqual(1, pagingSpan.GetTagItem("arvidsonfoto.page.number"));
        Assert.AreEqual(pageSpan.GetTagItem("arvidsonfoto.result.count"), pagingSpan.GetTagItem("arvidsonfoto.result.count"));
        var latestImage = processor.Activities.Single(activity =>
            activity.OperationName == "image.latest" && activity.Parent?.ParentSpanId == tooltipSpan.SpanId);
        Assert.AreEqual(pageSpan.TraceId, latestImage.TraceId);
        Assert.AreEqual(10, latestImage.GetTagItem("arvidsonfoto.category.requested_id"));
        Assert.IsNotNull(latestImage.GetTagItem("arvidsonfoto.image.path"));
        Assert.IsTrue(processor.Activities.Any(activity => activity.OperationName == "image.count" && activity.TraceId == pageSpan.TraceId));
        Assert.IsTrue(processor.Activities.Any(activity => activity.OperationName == "category.children" && activity.TraceId == pageSpan.TraceId));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task DevelopmentToolTraces_RespectOptIn(bool includeDevelopmentTools)
    {
        using var activityScope = new ActivityScope();
        using var processor = new RecordingProcessor();
        using var factory = new TraceFactory(processor, correlateImages: false,
            includeDevelopmentTools: includeDevelopmentTools).UseLocalKestrel();
        using var bootstrapClient = factory.CreateClient();
        var endpoint = bootstrapClient.BaseAddress!;
        foreach (var path in new[] { "/_vs/browserLink", "/_framework/aspnetcore-browser-refresh.js" })
        {
            _ = await GetRawAsync(endpoint, path);
            Assert.AreEqual(includeDevelopmentTools, processor.Activities.Any(activity =>
                activity.Kind == ActivityKind.Server && Equals(activity.GetTagItem("url.path"), path)));
        }
    }

    [TestMethod]
    public async Task DisabledBrowserCorrelation_DoesNotExposePageContext()
    {
        using var activityScope = new ActivityScope();
        using var processor = new RecordingProcessor();
        using var factory = new TraceFactory(processor, correlateImages: false, correlateBrowser: false).UseLocalKestrel();
        using var bootstrapClient = factory.CreateClient();
        var page = await GetRawAsync(bootstrapClient.BaseAddress!, "/");
        Assert.IsTrue(page.StartsWith("HTTP/1.1 200", StringComparison.Ordinal));
        Assert.IsNull(new HtmlParser().ParseDocument(page).QuerySelector("meta[name='arvidsonfoto-traceparent']"));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task JavaScriptTraces_RespectOptInWithoutChangingFileResponses(bool includeJavaScript)
    {
        using var activityScope = new ActivityScope();
        using var processor = new RecordingProcessor();
        using var factory = new TraceFactory(processor, correlateImages: false,
            includeJavaScript: includeJavaScript).UseLocalKestrel();
        using var bootstrapClient = factory.CreateClient();
        foreach (var path in new[]
        {
            "/js/pageTrace.js",
            "/js/categoryTooltip.js",
            "/lib/jquery-validation/dist/jquery.validate.min.js",
            "/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"
        })
        {
            var response = await GetRawAsync(bootstrapClient.BaseAddress!, path + "?v=unchanged");
            Assert.IsTrue(response.StartsWith("HTTP/1.1 200", StringComparison.Ordinal));
            Assert.AreEqual(includeJavaScript, processor.Activities.Any(activity =>
                activity.Kind == ActivityKind.Server && Equals(activity.GetTagItem("url.path"), path)));
        }
    }

    private static async Task<string> GetRawAsync(Uri endpoint, string path, string headers = "")
    {
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        using var client = new TcpClient();
        await client.ConnectAsync(endpoint.Host, endpoint.Port, cancellation.Token);
        using var stream = client.GetStream();
        var request = Encoding.ASCII.GetBytes(
            $"GET {path} HTTP/1.1\r\nHost: {endpoint.Authority}\r\n{headers}Connection: close\r\n\r\n");
        await stream.WriteAsync(request, cancellation.Token);
        using var reader = new StreamReader(stream, Encoding.Latin1, leaveOpen: true);
        return await reader.ReadToEndAsync(cancellation.Token);
    }

    private sealed class ActivityScope : IDisposable
    {
        private readonly Activity? _original = Activity.Current;

        public ActivityScope() => Activity.Current = null;

        public void Dispose() => Activity.Current = _original;
    }

    private sealed class RecordingProcessor : BaseProcessor<Activity>
    {
        internal ConcurrentBag<Activity> Activities { get; } = [];

        public override void OnEnd(Activity data) => Activities.Add(data);
    }

    private sealed class TraceFactory(RecordingProcessor processor, bool correlateImages,
        bool correlateBrowser = true, bool includeDevelopmentTools = false,
        bool includeJavaScript = false) : ArvidsonFotoWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Telemetry:CorrelateImageRequests"] = correlateImages.ToString(),
                    ["Telemetry:CorrelateBrowserRequests"] = correlateBrowser.ToString(),
                    ["Telemetry:IncludeDevelopmentToolTraces"] = includeDevelopmentTools.ToString(),
                    ["Telemetry:IncludeJavaScriptTraces"] = includeJavaScript.ToString()
                }));
            builder.ConfigureServices(services => services.AddOpenTelemetry()
                .WithTracing(tracing => tracing.SetSampler(new AlwaysOnSampler()).AddProcessor(processor)));
        }

        public TraceFactory UseLocalKestrel()
        {
            UseKestrel(0);
            return this;
        }
    }
}