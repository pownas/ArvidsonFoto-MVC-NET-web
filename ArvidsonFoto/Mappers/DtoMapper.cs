//using ArvidsonFoto.Core.DTOs;
//using ArvidsonFoto.Core.Models;
//using ArvidsonFoto.Services;

//namespace ArvidsonFoto.Mappers;

///// <summary>
///// Mapper functions for converting between Core.Models and Core.DTOs
///// </summary>
///// <remarks>
///// Denna ska vi försöka bygga bort... // Jonas, 2025-12-26
///// </remarks>
//public static class DtoMapper
//{
//    /// <summary>
//    /// Maps TblImage to ImageDto
//    /// </summary>
//    /// <remarks>
//    /// Denna ska vi försöka bygga bort... // Jonas, 2025-12-26
//    /// </remarks>
//    public static ImageDto MapToImageDto(TblImage image, ICategoryService categoryService)
//    {
//        return new ImageDto
//        {
//            ImageId = image.ImageId ?? -1,
//            Name = image.Name ?? categoryService.GetNameById(image.ImageCategoryId),
//            CategoryId = image.ImageCategoryId ?? -1,
//            Description = image.ImageDescription ?? string.Empty,
//            DateImageTaken = image.ImageDate,
//            DateUploaded = image.ImageUpdate,
//            UrlImage = image.ImageUrlName ?? string.Empty,
//            UrlCategory = BuildCategoryUrl(image, categoryService)
//        };
//    }

//    /// <summary>
//    /// Maps TblMenu to CategoryDto
//    /// </summary>
//    public static CategoryDto MapToCategoryDto(TblMenu menu)
//    {
//        return new CategoryDto
//        {
//            CategoryId = menu.MenuCategoryId,
//            Name = menu.MenuDisplayName,
//            ParentCategoryId = menu.MenuParentCategoryId,
//            UrlCategoryPath = menu.MenuUrlSegment ?? string.Empty,
//            DateUpdated = null // TblMenu doesn't have MenuUpdated field
//        };
//    }

//    /// <summary>
//    /// Builds category URL for an image
//    /// </summary>
//    private static string BuildCategoryUrl(TblImage image, ICategoryService categoryService)
//    {
//        var parts = new List<string> { "https://arvidsonfoto.se/Bilder" };

//        if (image.ImageMainFamilyId.HasValue)
//        {
//            var mainFamily = categoryService.GetNameById(image.ImageMainFamilyId);
//            if (!string.IsNullOrEmpty(mainFamily))
//                parts.Add(mainFamily);
//        }

//        if (image.ImageFamilyId.HasValue)
//        {
//            var family = categoryService.GetNameById(image.ImageFamilyId);
//            if (!string.IsNullOrEmpty(family))
//                parts.Add(family);
//        }

//        var category = categoryService.GetNameById(image.ImageCategoryId);
//        if (!string.IsNullOrEmpty(category))
//            parts.Add(category);

//        return string.Join("/", parts);
//    }
//}
