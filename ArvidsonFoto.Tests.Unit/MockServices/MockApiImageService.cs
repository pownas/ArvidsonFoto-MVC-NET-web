using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Data;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of IApiImageService for unit testing
/// Uses data from ArvidsonFotoCoreDbSeeder to match production data structure
/// </summary>
public class MockApiImageService : IApiImageService
{
    private readonly List<ImageDto> _testImages;
    private readonly MockApiCategoryService _categoryService;

    public MockApiImageService()
    {
        _categoryService = new MockApiCategoryService();
        
        // Convert ArvidsonFotoCoreDbSeeder image data to ImageDto format
        _testImages = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image.Select(image => new ImageDto
        {
            ImageId = image.ImageId ?? 0,
            CategoryId = image.ImageCategoryId ?? -1, // ImageCategoryId is the category ID
            Name = GetCategoryName(image.ImageCategoryId ?? -1),
            UrlImage = image.ImageUrlName ?? $"image-{image.ImageId}",
            UrlCategory = GetCategoryPath(image.ImageCategoryId ?? -1),
            DateImageTaken = image.ImageDate,
            DateUploaded = image.ImageUpdate,
            Description = image.ImageDescription ?? string.Empty
        }).ToList();
    }

    private string GetCategoryName(int categoryId)
    {
        if (categoryId <= 0) return "Unknown";
        var menu = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.FirstOrDefault(m => m.MenuCategoryId == categoryId);
        return menu?.MenuDisplayName ?? "Unknown";
    }

    private string GetCategoryPath(int categoryId)
    {
        if (categoryId <= 0) return "unknown";
        
        var pathParts = new List<string>();
        var currentMenuId = categoryId;
        
        while (currentMenuId > 0)
        {
            var menu = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_MenuCategories.FirstOrDefault(m => m.MenuCategoryId == currentMenuId);
            if (menu == null) break;
            
            pathParts.Insert(0, menu.MenuUrlSegment ?? $"category-{currentMenuId}");
            
            if (menu.MenuParentCategoryId == 0 || menu.MenuParentCategoryId == null)
                break;
                
            currentMenuId = menu.MenuParentCategoryId ?? 0;
        }
        
        return string.Join("/", pathParts);
    }

    public bool AddImage(ImageDto image)
    {
        if (image?.UrlImage == null) return false;
        
        var newId = _testImages.Count > 0 ? _testImages.Max(i => i.ImageId) + 1 : 1;
        image.ImageId = newId;
        image.DateUploaded = DateTime.UtcNow;
        _testImages.Add(image);
        return true;
    }

    public bool DeleteImgId(int imgId)
    {
        var image = _testImages.FirstOrDefault(i => i.ImageId == imgId);
        if (image == null) return false;
        
        _testImages.Remove(image);
        return true;
    }

    public int GetImageLastId()
    {
        return _testImages.Count > 0 ? _testImages.Max(i => i.ImageId) : 0;
    }

    public ImageDto GetOneImageFromCategory(int categoryId)
    {
        // Check if there are images directly in this category
        var image = _testImages
            .Where(i => i.CategoryId == categoryId)
            .OrderByDescending(i => i.DateUploaded)
            .FirstOrDefault();
            
        if (image != null) return image;
        
        // Check subcategories
        var subcategories = _categoryService.GetChildrenByParentId(categoryId);
        foreach (var subcategory in subcategories)
        {
            image = GetOneImageFromCategory(subcategory.CategoryId ?? -1);
            if (image.ImageId != -1) return image;
        }
            
        return CreateNotFoundImage();
    }

    public List<ImageDto> GetAll()
    {
        return new List<ImageDto>(_testImages);
    }

    public List<ImageDto> GetRandomNumberOfImages(int count)
    {
        return _testImages
            .OrderBy(i => Guid.NewGuid())
            .Take(count)
            .ToList();
    }

    public List<ImageDto> GetImagesByCategoryID(int categoryID)
    {
        return _testImages
            .Where(i => i.CategoryId == categoryID)
            .ToList();
    }

    public ImageDto GetById(int imageId)
    {
        if (imageId <= 0) return CreateNotFoundImage();
        
        var image = _testImages.FirstOrDefault(i => i.ImageId == imageId);
        return image ?? CreateNotFoundImage();
    }

    public Task<IEnumerable<ImageDto>> GetAllImagesAsync()
    {
        return Task.FromResult<IEnumerable<ImageDto>>(GetAll());
    }

    public Task<ImageDto> GetOneImageAsync(int id)
    {
        return Task.FromResult(GetById(id));
    }

    public Task<bool> CreateImageAsync(ImageDto image)
    {
        return Task.FromResult(AddImage(image));
    }

    public Task<bool> UpdateImageAsync(ImageDto image)
    {
        if (image?.ImageId == null) return Task.FromResult(false);
        
        var existingImage = _testImages.FirstOrDefault(i => i.ImageId == image.ImageId);
        if (existingImage == null) return Task.FromResult(false);
        
        existingImage.UrlImage = image.UrlImage;
        existingImage.CategoryId = image.CategoryId;
        existingImage.DateImageTaken = image.DateImageTaken;
        existingImage.Description = image.Description;
        existingImage.DateUploaded = DateTime.UtcNow;
        
        return Task.FromResult(true);
    }

    public Task<bool> DeleteImageAsync(int id)
    {
        return Task.FromResult(DeleteImgId(id));
    }

    public int GetCountedAllImages()
    {
        return _testImages.Count;
    }

    public int GetCountedCategoryId(int categoryId)
    {
        return _testImages.Count(i => i.CategoryId == categoryId);
    }

    public List<ImageDto> GetLatestImageList(int limit)
    {
        return _testImages
            .OrderByDescending(i => i.DateUploaded)
            .Take(limit)
            .ToList();
    }

    public ImageDto GetImageById(int id)
    {
        return GetById(id);
    }

    public bool DeleteImage(int id)
    {
        return DeleteImgId(id);
    }

    private static ImageDto CreateNotFoundImage()
    {
        return new ImageDto
        {
            ImageId = -1,
            CategoryId = -1,
            Name = "404-NotFound",
            UrlImage = "404-NotFound",
            UrlCategory = "bilder/404-NotFound",
            DateImageTaken = DateTime.Now,
            Description = "Bild inte hittad",
            DateUploaded = DateTime.Now
        };
    }
}