using ArvidsonFoto.Controllers;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;

public class UploadAdminFacebookImageUrlTests
{
    [Fact]
    public void BuildFacebookImageUrl_EncodesEachPathSegmentOnce()
    {
        var result = UploadAdminController.BuildFacebookImageUrl("Däggdjur/Räv", "file name.jpg");

        Assert.Equal("https://arvidsonfoto.se/bilder/D%C3%A4ggdjur/R%C3%A4v/file%20name.jpg", result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("../image.jpg")]
    [InlineData("folder/image.jpg")]
    [InlineData("folder\\image.jpg")]
    public void BuildFacebookImageUrl_RejectsInvalidFileNames(string? imageFileName)
    {
        var result = UploadAdminController.BuildFacebookImageUrl("Däggdjur/Räv", imageFileName);

        Assert.Null(result);
    }
}
