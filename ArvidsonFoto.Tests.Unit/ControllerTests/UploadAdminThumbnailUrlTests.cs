using ArvidsonFoto.Controllers;
using ArvidsonFoto.Core.DTOs;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;

public class UploadAdminThumbnailUrlTests
{
    [Theory]
    [InlineData("bilder/D%C3%A4ggdjur/R%C3%A4v/17TB7295", "/bilder/D%C3%A4ggdjur/R%C3%A4v/17TB7295.thumb.jpg")]
    [InlineData("bilder/Däggdjur/Björn/71TA5623", "/bilder/D%C3%A4ggdjur/Bj%C3%B6rn/71TA5623.thumb.jpg")]
    public void BuildThumbnailUrl_EncodesEachPathSegmentOnce(string imageUrl, string expectedUrl)
    {
        var result = UploadAdminController.BuildThumbnailUrl(new ImageDto
        {
            ImageId = 1,
            UrlImage = imageUrl
        });

        Assert.Equal(expectedUrl, result);
    }

    [Theory]
    [InlineData("bilder/%2E%2E/image")]
    [InlineData("bilder/folder%2F..%2Fimage")]
    public void BuildThumbnailUrl_RejectsTraversalSegments(string imageUrl)
    {
        var result = UploadAdminController.BuildThumbnailUrl(new ImageDto
        {
            ImageId = 1,
            UrlImage = imageUrl
        });

        Assert.Null(result);
    }
}
