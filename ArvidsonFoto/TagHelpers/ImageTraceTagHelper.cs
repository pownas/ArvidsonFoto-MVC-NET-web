using ArvidsonFoto.Core.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ArvidsonFoto.TagHelpers;

[HtmlTargetElement("img", Attributes = "src", TagStructure = TagStructure.WithoutEndTag)]
[HtmlTargetElement("a", Attributes = "href, data-gallery")]
public sealed class ImageTraceTagHelper : TagHelper
{
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    // Run after the framework's URL-resolution and versioning helpers.
    public override int Order => 1000;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);
        var traceContext = ViewContext.HttpContext.RequestServices.GetRequiredService<ImageTraceContext>();
        var attributeName = output.TagName == "img" ? "src" : "href";
        var attribute = output.Attributes[attributeName];
        var url = attribute?.Value as string;
        if (attribute?.Value is IHtmlContent htmlContent)
        {
            using var writer = new StringWriter(CultureInfo.InvariantCulture);
            htmlContent.WriteTo(writer, HtmlEncoder.Default);
            url = WebUtility.HtmlDecode(writer.ToString());
        }

        if (url is not null)
        {
            var tracedUrl = traceContext.AddToLocalUrl(ViewContext.HttpContext, url);
            if (tracedUrl != url)
            {
                output.Attributes.SetAttribute(attributeName, tracedUrl);
            }
        }
    }
}