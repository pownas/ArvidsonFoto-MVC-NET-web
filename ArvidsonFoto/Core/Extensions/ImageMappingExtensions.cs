using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Core.Extensions;

/// <summary>
/// Extension methods for mapping between TblImage and ImageDto
/// </summary>
public static class ImageMappingExtensions
{
    /// <summary>
    /// Converts a TblImage to an ImageDto
    /// </summary>
    /// <param name="image">The TblImage to convert</param>
    /// <param name="categoryPath">Optional category path for URL generation</param>
    /// <returns>An ImageDto representation of the TblImage</returns>
    public static ImageDto ToImageDto(this TblImage image, string categoryPath = "")
    {
        if (image == null)
        {
            return new ImageDto();
        }

        var imgUrl = string.IsNullOrEmpty(categoryPath)
            ? $"bilder/{image.ImageUrlName}"
            : $"bilder/{categoryPath}/{image.ImageUrlName}";

        return new ImageDto
        {
            ImageId = image.ImageId ?? 0,
            CategoryId = image.ImageCategoryId ?? -1,
            UrlImage = imgUrl,
            UrlCategorySv = string.IsNullOrEmpty(categoryPath) ? string.Empty : $"bilder/{categoryPath}",
            UrlCategoryEn = string.Empty,
            DateImageTaken = image.ImageDate,
            DateUploaded = image.ImageUpdate,
            DescriptionSv = image.ImageDescription ?? string.Empty,
            DescriptionEn = image.ImageDescriptionEn,
            IsGameSelectable = image.IsGameSelectable
        };
    }

    /// <summary>
    /// Converts an ImageDto back to TblImage for database operations
    /// </summary>
    /// <param name="imageDto">The ImageDto to convert</param>
    /// <returns>A TblImage representation of the ImageDto</returns>
    public static TblImage ToTblImage(this ImageDto imageDto)
    {
        var imageFileName = imageDto?.UrlImage?.Split('/').LastOrDefault();
        return imageDto == null
            ? new TblImage()
            : new TblImage
        {
            ImageId = imageDto.ImageId,
            ImageCategoryId = imageDto.CategoryId == -1 ? null : imageDto.CategoryId,
            ImageUrlName = imageFileName,
            ImageDate = imageDto.DateImageTaken,
            ImageUpdate = imageDto.DateUploaded ?? DateTime.UtcNow,
            ImageDescription = imageDto.DescriptionSv,
            ImageDescriptionEn = imageDto.DescriptionEn,
            IsGameSelectable = imageDto.IsGameSelectable
        };
    }
}