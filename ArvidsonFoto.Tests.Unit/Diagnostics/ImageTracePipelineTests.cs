using System.Diagnostics;
using ArvidsonFoto.Core.Diagnostics;
using ArvidsonFoto.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArvidsonFoto.Tests.Unit.Diagnostics;

public class ImageTracePipelineTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TagHelper_HandlesStringAndRazorEncodedUrlWithoutLosingVersion(bool encoded)
    {
        using var services = new ServiceCollection().AddSingleton(new ImageTraceContext(true)).BuildServiceProvider();
        var httpContext = CreateContext();
        httpContext.RequestServices = services;
        using var request = new Activity("request").SetIdFormat(ActivityIdFormat.W3C).Start();
        request.ActivityTraceFlags = ActivityTraceFlags.Recorded;
        var helper = new ImageTraceTagHelper { ViewContext = new ViewContext { HttpContext = httpContext } };
        object url = encoded
            ? new HtmlString("/img/photo.jpg?v=7&amp;size=small")
            : "/img/photo.jpg?v=7&size=small";
        var attributes = new TagHelperAttributeList { new("src", url) };
        var context = new TagHelperContext(attributes, new Dictionary<object, object>(), "image");
        var output = new TagHelperOutput("img", attributes,
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        helper.Process(context, output);

        var result = Assert.IsType<string>(output.Attributes["src"].Value);
        var query = QueryHelpers.ParseQuery(new Uri(new Uri("https://localhost/"), result).Query);
        Assert.Equal("7", query["v"].ToString());
        Assert.Equal("small", query["size"].ToString());
        Assert.Equal(request.Id, query[ImageTraceContext.QueryParameter].ToString());
        Assert.Equal(3, query.Count);
    }

    [Theory]
    [InlineData(200, ActivityStatusCode.Unset)]
    [InlineData(304, ActivityStatusCode.Unset)]
    [InlineData(404, ActivityStatusCode.Unset)]
    [InlineData(500, ActivityStatusCode.Error)]
    public async Task Middleware_UsesPageContextAndRecordsActualResponse(int statusCode, ActivityStatusCode expectedStatus)
    {
        Activity? imageSpan = null;
        using var listener = CreateListener(activity => imageSpan = activity);
        using var hosting = new Activity("independent hosting span").Start();
        var context = CreateContext();
        var traceId = ActivityTraceId.CreateRandom();
        var spanId = ActivitySpanId.CreateRandom();
        context.Request.QueryString = new QueryString($"?af_traceparent=00-{traceId}-{spanId}-01");
        var middleware = new ImageTraceMiddleware(httpContext =>
        {
            Assert.Equal(traceId, Activity.Current?.TraceId);
            Assert.Equal(spanId, Activity.Current?.ParentSpanId);
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "image/jpeg";
            httpContext.Response.ContentLength = 123;
            return Task.CompletedTask;
        }, new ImageTraceContext(true), NullLogger<ImageTraceMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.NotNull(imageSpan);
        Assert.Equal(ActivityKind.Server, imageSpan.Kind);
        Assert.Equal(expectedStatus, imageSpan.Status);
        Assert.Equal(statusCode, imageSpan.GetTagItem("http.response.status_code"));
        Assert.Equal(123L, imageSpan.GetTagItem("http.response.body.size"));
        Assert.Equal("image/jpeg", imageSpan.GetTagItem("http.response.header.content-type"));
        Assert.Equal(hosting, Activity.Current);
    }

    [Fact]
    public async Task Middleware_RecordsExceptionAndRethrowsWithoutExposingExceptionMessage()
    {
        Activity? imageSpan = null;
        using var listener = CreateListener(activity => imageSpan = activity);
        var context = CreateContext();
        context.Request.QueryString = new QueryString($"?af_traceparent=00-{ActivityTraceId.CreateRandom()}-{ActivitySpanId.CreateRandom()}-01");
        var exception = new InvalidOperationException("private-test-message");
        var middleware = new ImageTraceMiddleware(_ => throw exception,
            new ImageTraceContext(true), NullLogger<ImageTraceMiddleware>.Instance);

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));

        Assert.Same(exception, thrown);
        Assert.NotNull(imageSpan);
        Assert.Equal(ActivityStatusCode.Error, imageSpan.Status);
        Assert.Equal(typeof(InvalidOperationException).FullName, imageSpan.GetTagItem("error.type"));
        var error = Assert.Single(imageSpan.Events);
        Assert.Equal("exception", error.Name);
        Assert.DoesNotContain(error.Tags, tag => tag.Key == "exception.message");
        Assert.Contains(error.Tags, tag => tag.Key == "exception.type" && Equals(tag.Value, typeof(InvalidOperationException).FullName));
    }

    private static ActivityListener CreateListener(Action<Activity> stopped)
    {
        var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "ArvidsonFoto",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity =>
            {
                if (activity.OperationName == "image.fetch /img/photo.jpg")
                {
                    stopped(activity);
                }
            }
        };
        ActivitySource.AddActivityListener(listener);
        return listener;
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("localhost");
        context.Request.Path = "/img/photo.jpg";
        return context;
    }
}