using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Extensions;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Core.Services;

/// <summary>
/// Provides functionality for managing images, including adding, retrieving, updating, and deleting images.
/// </summary>
/// <remarks>The <see cref="ApiImageService"/> class interacts with the database to perform operations on image data.
/// It includes methods for retrieving images by category, fetching random images, and managing image metadata. This
/// service is designed to handle common image-related operations for the application.</remarks>
/// <param name="logger">Logging for the service.</param>
/// <param name="dbContext"><see cref="ArvidsonFotoCoreDbContext"/> instance used to interact with the database.</param>
/// <param name="_configuration"><see cref="IConfiguration"/> instance used to access application settings, such as feature flags.</param>
/// <param name="categoryService">The category service for category path operations.</param>
public class ApiImageService(ILogger<ApiImageService> logger, ArvidsonFotoCoreDbContext dbContext, IConfiguration _configuration, IApiCategoryService apiCategoryService) : IApiImageService
{
    /// <summary> Databas koppling: <see cref="ArvidsonFotoCoreDbContext"/> </summary>
    private readonly ArvidsonFotoCoreDbContext _entityContext = dbContext;

    /// <summary>
    /// If the new gallery category feature is enabled, use the new category path
    /// </summary>
    private bool NewGalleryCategoryEnabled => 
        _configuration.GetSection("FeatureFlags:NewGalleryCategory")
            .Get<FeatureFlag>()?.Enabled == true;

    /// <summary> Värde när <see cref="ImageDto"/> inte hittats </summary>
    private static ImageDto DefaultImageDtoNotFound { get; } = new()
    {
        ImageId = -1,
        CategoryId = -1,
        Name = "404-NotFound", //Ett standardvärde som kan användas i UI, om ingen bild hittats.
        UrlImage = "bilder/404-NotFound",
        UrlCategory = string.Empty,
        DateImageTaken = DateTime.Now,
        Description = "Bild inte hittad",
        DateUploaded = DateTime.Now,
    };

    /// <summary> Värde när <see cref="TblImage"/> inte hittats </summary>
    private static TblImage DefaultTblImageNotFound { get; } = new()
    {
        Id = null, // Id is not used in the DTO, so we set it to null
        ImageId = -1,
        ImageCategoryId = -1,
        ImageFamilyId = -1,
        ImageMainFamilyId = -1,
        ImageUrlName = "404-NotFound", //Ett standardvärde som kan användas i UI, om ingen bild hittats.
        ImageDate = DateTime.Now,
        ImageDescription = "Bild inte hittad",
        ImageUpdate = DateTime.Now,
        Name = "404-NotFound", //Ett standardvärde som kan användas i UI, om ingen bild hittats.
    };

    /// <summary>
    /// Adds an image to the database.
    /// </summary>
    /// <param name="image">The image data to be added, represented as an <see cref="ImageDto"/> object.</param>
    /// <returns><see langword="true"/> if the image was successfully added to the database; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="Exception">Thrown if an error occurs while adding the image to the database. The exception message contains details about
    /// the error.</exception>
    public bool AddImage(ImageDto image)
    {
        bool success; //default är false
        try
        {
            var tblImage = image.ToTblImage();
            _entityContext.TblImages.Add(tblImage);
            _entityContext.SaveChanges();
            success = true;
        }
        catch (Exception ex)
        {
            string ErrorMessage = "Fel vid länkning av bild. Felmeddelande: " + ex.Message;

            Log.Warning(ErrorMessage);
            throw new Exception(ErrorMessage);
        }
        return success;
    }

    /// <summary>
    /// Deletes an image record with the specified ID from the database.
    /// </summary>
    /// <remarks>If no image with the specified ID exists, the method returns <see langword="false"/> without
    /// performing any deletion. Logs an error if an exception occurs during the operation.</remarks>
    /// <param name="imgId">The unique identifier of the image to delete.</param>
    /// <returns><see langword="true"/> if the image was successfully deleted; otherwise, <see langword="false"/>.</returns>
    public bool DeleteImgId(int imgId)
    {
        logger.LogWarning("DeleteImgId called for imgId: {ImgId}", imgId);
        bool succeeded = false; //verkar som det måste heta "success" för att defaulta till false. För det går inte att ta bort false tilldelningen.
        try
        {
            TblImage? image = _entityContext.TblImages.Where(i => i.ImageId == imgId)
                                                    .FirstOrDefault();
            if (image != null)
            {
                _entityContext.TblImages.Remove(image);
                _entityContext.SaveChanges();
                succeeded = true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Error when deleting the image with id: " + imgId + ". Error-message: " + ex.Message);
        }
        return succeeded;
    }

    /// <summary>
    /// Gets the ID of the last added image.
    /// </summary>
    /// <returns>The ID of the last image, or -1 if no images exist or an error occurs.</returns>
    public int GetImageLastId()
    {
        try
        {
            var lastImage = _entityContext.TblImages.OrderByDescending(i => i.ImageId).FirstOrDefault();
            return lastImage?.ImageId ?? -1;
        }
        catch (Exception ex)
        {
            Log.Error("GetImageLastId failed: {Message}", ex.Message);
            return -1;
        }
    }

    /// <summary>
    /// Gets one image from a specific category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to get an image from.</param>
    /// <returns>An image DTO from the specified category, or a default "not found" image if no image exists.</returns>
    public ImageDto GetOneImageFromCategory(int categoryId)
    {
        try
        {
            TblImage image;
            if (categoryId == 1) //Om man söker fram Id = 1 (Fåglar) , så ska Id för Blåmes hittas och visas bilden för istället. 
            {
                var blames = _entityContext.TblMenus
                                           .Where(m => m.MenuDisplayName!.Equals("Blåmes"))
                                           .FirstOrDefault();
                                           
                if (blames == null)
                {
                    Log.Warning("Blåmes category not found when getting image for category ID 1");
                    return DefaultImageDtoNotFound;
                }
                                           
                int blamesId = blames.MenuCategoryId ?? 0;

                image = _entityContext.TblImages
                                     .Where(i => i.ImageCategoryId.Equals(blamesId))
                                     .OrderByDescending(i => i.ImageUpdate)
                                     .FirstOrDefault() ?? DefaultTblImageNotFound;
            }
            else
            {
                image = _entityContext.TblImages
                                     .Where(i => i.ImageCategoryId.Equals(categoryId)
                                              || i.ImageFamilyId.Equals(categoryId)
                                              || i.ImageMainFamilyId.Equals(categoryId))
                                     .OrderByDescending(i => i.ImageUpdate)
                                     .FirstOrDefault() ?? DefaultTblImageNotFound;
            }

            var categoryPath = "";
            if (NewGalleryCategoryEnabled)
            {
                categoryPath = apiCategoryService.GetCategoryPathForImage(image.ImageCategoryId ?? -1);
            }
            else
            {
                // Otherwise, use the old category path
                categoryPath = GetOldCategoryPathForImage(image);
            }

            return image.ToImageDto(categoryPath) ?? DefaultImageDtoNotFound;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetOneImageFromCategory: {Message}", ex.Message);
            return DefaultImageDtoNotFound;
        }
    }

    /// <summary>
    /// Gets all images from the database.
    /// </summary>
    /// <returns>A list of all images as DTOs.</returns>
    public List<ImageDto> GetAll()
    {
        var images = _entityContext.TblImages.ToList();

        // Get category paths for all images
        var imageDtos = new List<ImageDto>();
        foreach (var image in images)
        {
            // If the new gallery category feature is enabled, use the new category path
            var featureNewGalleryCategory = _configuration.GetSection("FeatureFlags:NewGalleryCategory").Get<FeatureFlag>();
            var categoryPath = "";
            if (featureNewGalleryCategory?.Enabled == true)
            {
                categoryPath = apiCategoryService.GetCategoryPathForImage(image.ImageCategoryId ?? -1);
            }
            else
            {
                // Otherwise, use the old category path
                categoryPath = GetOldCategoryPathForImage(image);
            }

            imageDtos.Add(image.ToImageDto(categoryPath));
        }
        return imageDtos;
    }

    /// <summary> Används på startsidan för random antal av bilder. </summary>
    /// <param name="count">Antal bilder som ska plockas ifrån databasen</param>
    /// <returns>"count" antal bilder ifrån databasen</returns>
    public List<ImageDto> GetRandomNumberOfImages(int count)
    {
        var images = _entityContext.TblImages
                               .OrderBy(r => Guid.NewGuid()) //Här gör jag en random med hjälp av en ny GUID som random nummer.
                               .Take(count)
                               .ToList();

        // Get category paths for all images
        var imageDtos = new List<ImageDto>();
        foreach (var image in images)
        {
            var categoryPath = "";
            var categoryName = "";
            
            if (NewGalleryCategoryEnabled)
            {
                categoryPath = apiCategoryService.GetCategoryPathForImage(image.ImageCategoryId ?? -1);
            }
            else
            {
                // Otherwise, use the old category path
                categoryPath = GetOldCategoryPathForImage(image);
            }
            
            // Get category name for display
            categoryName = apiCategoryService.GetNameById(image.ImageCategoryId);

            imageDtos.Add(image.ToImageDto(categoryPath, categoryName));
        }

        return imageDtos;
    }

    /// <summary>
    /// Gets a list of images by category ID
    /// </summary>
    /// <param name="categoryID">The ID of the category to find images for</param>
    /// <returns>List of images in the specified category</returns>
    public List<ImageDto> GetImagesByCategoryID(int categoryID)
    {
        try
        {
            if (categoryID <= 0)
            {
                Log.Information("Invalid category ID for GetImagesByCategoryID: {CategoryID}", categoryID);
                return new List<ImageDto>();
            }
            
            var images = _entityContext.TblImages
                        .Where(i => i.ImageCategoryId == categoryID
                                 || i.ImageFamilyId == categoryID
                                 || i.ImageMainFamilyId == categoryID)
                        .ToList();

            // Early return if no images found
            if (!images.Any())
            {
                return new List<ImageDto>();
            }

            // Check feature flag once
            var featureNewGalleryCategory = _configuration.GetSection("FeatureFlags:NewGalleryCategory").Get<FeatureFlag>();
            
            if (featureNewGalleryCategory?.Enabled == true)
            {
                // OPTIMIZED: Bulk load all category paths at once
                return GetImageDtosWithOptimizedCategoryPaths(images);
            }
            else
            {
                // Use old category path logic with category name
                var imageDtos = new List<ImageDto>();
                foreach (var image in images)
                {
                    var categoryPath = GetOldCategoryPathForImage(image);
                    // Get the actual category name for THIS image, not the search category
                    var categoryName = apiCategoryService.GetNameById(image.ImageCategoryId ?? -1);
                    imageDtos.Add(image.ToImageDto(categoryPath, categoryName));
                }
                return imageDtos;
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetImagesByCategoryID: {Message}", ex.Message);
            return new List<ImageDto>();
        }
    }

    /// <summary>
    /// Gets a paginated list of images by category ID with sorting
    /// </summary>
    /// <param name="categoryID">The ID of the category to find images for</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of images per page</param>
    /// <returns>Paginated list of images in the specified category</returns>
    public List<ImageDto> GetImagesByCategoryIDPaginated(int categoryID, int page, int pageSize)
    {
        try
        {
            if (categoryID <= 0)
            {
                Log.Information("Invalid category ID for GetImagesByCategoryIDPaginated: {CategoryID}", categoryID);
                return new List<ImageDto>();
            }
            
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 48;
            
            // OPTIMIZED: Get only the images we need with sorting applied in SQL
            var images = _entityContext.TblImages
                        .Where(i => i.ImageCategoryId == categoryID
                                 || i.ImageFamilyId == categoryID
                                 || i.ImageMainFamilyId == categoryID)
                        .OrderByDescending(i => i.ImageId)
                        .ThenByDescending(i => i.ImageDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            // Early return if no images found
            if (!images.Any())
            {
                return new List<ImageDto>();
            }

            // Check feature flag once
            var featureNewGalleryCategory = _configuration.GetSection("FeatureFlags:NewGalleryCategory").Get<FeatureFlag>();
            
            if (featureNewGalleryCategory?.Enabled == true)
            {
                // OPTIMIZED: Bulk load all category paths at once
                return GetImageDtosWithOptimizedCategoryPaths(images);
            }
            else
            {
                // Use old category path logic with category name
                var imageDtos = new List<ImageDto>();
                foreach (var image in images)
                {
                    var categoryPath = GetOldCategoryPathForImage(image);
                    // Get the actual category name for THIS image, not the search category
                    var categoryName = apiCategoryService.GetNameById(image.ImageCategoryId ?? -1);
                    imageDtos.Add(image.ToImageDto(categoryPath, categoryName));
                }
                return imageDtos;
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetImagesByCategoryIDPaginated: {Message}", ex.Message);
            return new List<ImageDto>();
        }
    }

    /// <summary>
    /// Gets an image by its ID
    /// </summary>
    /// <param name="imageId">The ID of the image to retrieve</param>
    /// <returns>The image with the specified ID or a default image if not found</returns>
    public ImageDto GetById(int imageId)
    {
        try
        {
            if (imageId <= 0)
            {
                Log.Information("Invalid image ID for GetById: {ImageId}", imageId);
                return DefaultImageDtoNotFound;
            }

            var image = _entityContext.TblImages
                            .Where(i => i.ImageId == imageId)
                            .FirstOrDefault() ?? DefaultTblImageNotFound;

            var categoryPath = "";
            if (NewGalleryCategoryEnabled)
            {
                categoryPath = apiCategoryService.GetCategoryPathForImage(image.ImageCategoryId ?? -1);
            }
            else
            {
                // Otherwise, use the old category path
                categoryPath = GetOldCategoryPathForImage(image);
            }

            // Convert the image to DTO and include the category path
            return image.ToImageDto(categoryPath) ?? DefaultImageDtoNotFound;
        }
        catch (Exception ex)
        {
            Log.Error("Error in GetById: {Id} {Message}", imageId, ex.Message);
            return DefaultImageDtoNotFound;
        }
    }
    
    // Extension-compatible methods
    /// <summary>
    /// Gets all images from the database asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of image DTOs.</returns>
    public Task<IEnumerable<ImageDto>> GetAllImagesAsync()
    {
        return Task.FromResult<IEnumerable<ImageDto>>(GetAll());
    }
    
    /// <summary>
    /// Gets a single image by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the image to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the image DTO.</returns>
    public Task<ImageDto> GetOneImageAsync(int id)
    {
        return Task.FromResult(GetById(id));
    }
    
    /// <summary>
    /// Creates a new image asynchronously.
    /// </summary>
    /// <param name="image">The image to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the creation was successful.</returns>
    public Task<bool> CreateImageAsync(ImageDto image)
    {
        return Task.FromResult(AddImage(image));
    }
    
    /// <summary>
    /// Updates an existing image asynchronously.
    /// </summary>
    /// <param name="image">The image data to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the update was successful.</returns>
    public async Task<bool> UpdateImageAsync(ImageDto image)
    {
        try
        {
            var tblImage = image.ToTblImage();
            var existingImage = _entityContext.TblImages
                .Where(i => i.ImageId == tblImage.ImageId)
                .FirstOrDefault();
            if (existingImage != null)
            {
                existingImage.ImageUrlName = tblImage.ImageUrlName;
                existingImage.ImageCategoryId = tblImage.ImageCategoryId;
                existingImage.ImageFamilyId = tblImage.ImageFamilyId;
                existingImage.ImageMainFamilyId = tblImage.ImageMainFamilyId;
                existingImage.ImageDate = tblImage.ImageDate;
                existingImage.ImageDescription = tblImage.ImageDescription;
                existingImage.ImageUpdate = DateTime.UtcNow;
                
                await _entityContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Log.Error($"Fel vid uppdatering av bild. Felmeddelande: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Deletes an image by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the image to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the deletion was successful.</returns>
    public Task<bool> DeleteImageAsync(int id)
    {
        return Task.FromResult(DeleteImgId(id));
    }
    
    /// <summary>
    /// Gets the total count of all images.
    /// </summary>
    /// <returns>The total number of images.</returns>
    public int GetCountedAllImages()
    {
        return _entityContext.TblImages.Count();
    }

    /// <summary>
    /// Gets the count of all images in a specified categoryId.
    /// </summary>
    /// <returns>A The total number of images for the.</returns>
    public int GetCountedCategoryId(int categoryId)
    {
        var imagesCategoryCount = _entityContext.TblImages.Count(x => x.ImageCategoryId == categoryId);
        var imagesFamilyCount = _entityContext.TblImages.Count(x => x.ImageFamilyId == categoryId);
        var imagesMainFamilyCount = _entityContext.TblImages.Count(x => x.ImageMainFamilyId == categoryId);
        var totalImagesForCategoryId = imagesCategoryCount + imagesFamilyCount + imagesMainFamilyCount;
        return totalImagesForCategoryId;
    }

    /// <summary>
    /// Gets a list of the latest images limited by the specified count.
    /// </summary>
    /// <param name="limit">The maximum number of images to return.</param>
    /// <returns>A list of the latest images as DTOs.</returns>
    public List<ImageDto> GetLatestImageList(int limit)
    {
        var images = _entityContext.TblImages
            .OrderByDescending(i => i.ImageUpdate)
            .Take(limit)
            .ToList();
        
        // Get category paths for all images
        var imageDtos = new List<ImageDto>();
        foreach (var image in images)
        {
            var categoryPath = "";
            if (NewGalleryCategoryEnabled)
            {
                categoryPath = apiCategoryService.GetCategoryPathForImage(image.ImageCategoryId ?? -1);
            }
            else
            {
                // Otherwise, use the old category path
                categoryPath = GetOldCategoryPathForImage(image);
            }

            imageDtos.Add(image.ToImageDto(categoryPath));
        }
        
        return imageDtos;
    }

    /// <summary>
    /// Old category paths for images
    /// </summary>
    /// <param name="image">Image model in db</param>
    /// <returns>bilder/hackspettar/tretåig-hackspett , utan "/fåglar" och med ÅÄÖ</returns>
    private string GetOldCategoryPathForImage(TblImage image)
    {
        if (image.ImageCategoryId == null || image.ImageCategoryId <= 0)
            return string.Empty;

        // Build the category path by traversing up the parent chain
        var segments = new List<string>();
        var currentId = image.ImageCategoryId;

        // Start with the current category and traverse up to the root category
        while (currentId != null && currentId > 0)
        {
            // Fetch the current category and its parent category from the database
            var category = _entityContext.TblMenus
                .Where(c => c.MenuCategoryId == currentId)
                .Select(c => new { c.MenuParentCategoryId, c.MenuDisplayName })
                .FirstOrDefault();

            if (currentId == 1)
                break; // If the category is "Fåglar", return empty string

            // If the category is not found, break the loop
            if (category == null)
                break;

            // Insert the URL segment at the beginning of the list
            if (!string.IsNullOrWhiteSpace(category.MenuDisplayName))
                segments.Insert(0, category.MenuDisplayName);

            // Move to the parent category
            currentId = category.MenuParentCategoryId;
        }

        return segments.Count > 0 ? string.Join("/", segments).ToLowerInvariant() : string.Empty;
    }

    /// <summary>
    /// Optimized method to get ImageDtos with category paths using bulk loading
    /// </summary>
    private List<ImageDto> GetImageDtosWithOptimizedCategoryPaths(List<TblImage> images)
    {
        // Get all unique category IDs from the images
        var categoryIds = images
            .Select(i => i.ImageCategoryId)
            .Where(id => id.HasValue && id.Value > 0)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        // Bulk load category paths for all unique category IDs
        var categoryPaths = apiCategoryService.GetCategoryPathsBulk(categoryIds);

        // Build ImageDtos with cached category paths and category names
        var imageDtos = new List<ImageDto>();
        foreach (var image in images)
        {
            var categoryPath = "";
            var categoryName = "";
            if (image.ImageCategoryId.HasValue && categoryPaths.TryGetValue(image.ImageCategoryId.Value, out var path))
            {
                categoryPath = path;
                categoryName = apiCategoryService.GetNameById(image.ImageCategoryId.Value);
            }
            imageDtos.Add(image.ToImageDto(categoryPath, categoryName));
        }

        return imageDtos;
    }
    
    /// <summary>
    /// Gets an image by its ID.
    /// </summary>
    /// <param name="id">The ID of the image to retrieve.</param>
    /// <returns>The image DTO if found, otherwise a default "not found" image.</returns>
    public ImageDto GetImageById(int id)
    {
        return GetById(id);
    }
    
    /// <summary>
    /// Deletes an image by its ID.
    /// </summary>
    /// <param name="id">The ID of the image to delete.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    public bool DeleteImage(int id)
    {
        return DeleteImgId(id);
    }
}