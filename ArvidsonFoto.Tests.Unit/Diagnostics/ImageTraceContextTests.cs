using System.Diagnostics;
using ArvidsonFoto.Core.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;

namespace ArvidsonFoto.Tests.Unit.Diagnostics;

public class ImageTraceContextTests
{
    [Theory]
    [InlineData("/img/photo.jpg")]
    [InlineData("./img/photo.PNG")]
    [InlineData("~/img/photo.gif")]
    [InlineData("https://localhost/img/photo.webp?v=7#preview")]
    public void LocalImageUrl_UsesRequestSpanRatherThanRenderingSpan(string url)
    {
        var context = CreateContext();
        using var request = new Activity("request").SetIdFormat(ActivityIdFormat.W3C).Start();
        request.ActivityTraceFlags = ActivityTraceFlags.Recorded;
        context.Features.Set<IHttpActivityFeature>(new TestActivityFeature { Activity = request });
        using var rendering = new Activity("render").Start();

        var result = new ImageTraceContext(true).AddToLocalUrl(context, url);

        var absolute = new Uri(new Uri("https://localhost/"), result.Replace("~/", "", StringComparison.Ordinal));
        var query = QueryHelpers.ParseQuery(absolute.Query);
        Assert.Equal(request.Id, query[ImageTraceContext.QueryParameter].ToString());
        Assert.NotEqual(rendering.Id, query[ImageTraceContext.QueryParameter].ToString());
        if (url.Contains("v=7", StringComparison.Ordinal))
        {
            Assert.Equal("7", query["v"].ToString());
            Assert.Equal("#preview", absolute.Fragment);
        }
    }

    [Theory]
    [InlineData("https://arvidsonfoto.se/bilder/photo.jpg")]
    [InlineData("//other.example/img/photo.jpg")]
    [InlineData("http://localhost/img/photo.jpg")]
    [InlineData("data:image/png;base64,AAAA")]
    [InlineData("/bilder/faglar")]
    [InlineData("/img/photo.jpg?af_traceparent=existing")]
    public void ExternalOrNonImageUrl_IsUnchanged(string url)
    {
        using var request = new Activity("request").SetIdFormat(ActivityIdFormat.W3C).Start();
        request.ActivityTraceFlags = ActivityTraceFlags.Recorded;
        Assert.Equal(url, new ImageTraceContext(true).AddToLocalUrl(CreateContext(), url));
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void DisabledOrUnsampledTrace_DoesNotChangeCacheKey(bool enabled, bool recorded)
    {
        using var request = new Activity("request").SetIdFormat(ActivityIdFormat.W3C).Start();
        request.ActivityTraceFlags = recorded ? ActivityTraceFlags.Recorded : ActivityTraceFlags.None;
        const string url = "/img/photo.jpg?v=7";
        Assert.Equal(url, new ImageTraceContext(enabled).AddToLocalUrl(CreateContext(), url));
    }

    [Theory]
    [InlineData("GET", "/img/photo.jpg", "invalid")]
    [InlineData("GET", "/bilder/faglar", "00-11111111111111111111111111111111-2222222222222222-01")]
    [InlineData("POST", "/img/photo.jpg", "00-11111111111111111111111111111111-2222222222222222-01")]
    [InlineData("GET", "/img/photo.jpg", "00-00000000000000000000000000000000-2222222222222222-01")]
    public void InvalidOrNonImageContext_IsNotUsed(string method, string path, string parent)
    {
        var context = CreateContext();
        context.Request.Method = method;
        context.Request.Path = path;
        context.Request.QueryString = QueryString.Create(ImageTraceContext.QueryParameter, parent);
        Assert.False(new ImageTraceContext(true).TryGetParent(context.Request, out _));
    }

    [Fact]
    public void ValidContext_IsRemoteAndPreservesTraceAndParentIds()
    {
        var context = CreateContext();
        context.Request.Path = "/img/photo.jpg";
        const string value = "00-11111111111111111111111111111111-2222222222222222-01";
        context.Request.QueryString = QueryString.Create(ImageTraceContext.QueryParameter, value);

        Assert.True(new ImageTraceContext(true).TryGetParent(context.Request, out var parent));
        Assert.Equal("11111111111111111111111111111111", parent.TraceId.ToHexString());
        Assert.Equal("2222222222222222", parent.SpanId.ToHexString());
        Assert.True(parent.IsRemote);
        Assert.Equal(ActivityTraceFlags.Recorded, parent.TraceFlags);

        context.Request.QueryString = new QueryString($"?af_traceparent={value}&af_traceparent={value}");
        Assert.False(new ImageTraceContext(true).TryGetParent(context.Request, out _));
    }

    private sealed class TestActivityFeature : IHttpActivityFeature
    {
        public Activity Activity { get; set; } = null!;
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("localhost");
        return context;
    }
}