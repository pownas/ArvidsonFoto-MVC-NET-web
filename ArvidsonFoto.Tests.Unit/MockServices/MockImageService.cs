using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of IApiImageService for unit testing
/// Returns ImageDto objects directly
/// </summary>
public class MockImageService : IApiImageService
{
    public bool AddImage(ImageDto image) => true;
    
    public ImageDto GetById(int imageId) => new ImageDto 
    { 
        ImageId = imageId, 
        Name = "Test Image",
        UrlImage = "bilder/test/test-image",
        CategoryId = 1
    };
    
    public ImageDto GetOneImageFromCategory(int categoryId) => new ImageDto 
    { 
        ImageId = 1, 
        CategoryId = categoryId,
        Name = "Test Category Image",
        UrlImage = "bilder/test/category-image"
    };
    
    public List<ImageDto> GetAll() => new List<ImageDto> 
    { 
        new ImageDto 
        { 
            ImageId = 1, 
            Name = "Test Image 1",
            UrlImage = "bilder/test/image1",
            CategoryId = 1
        } 
    };
    
    public List<ImageDto> GetImagesByCategoryID(int categoryID) => new List<ImageDto> 
    { 
        new ImageDto 
        { 
            ImageId = 1, 
            CategoryId = categoryID,
            Name = "Test Image",
            UrlImage = "bilder/test/image"
        } 
    };
    
    public bool DeleteImgId(int imgId) => true;
    
    public int GetImageLastId() => 100;
    
    public List<ImageDto> GetRandomNumberOfImages(int count) => Enumerable.Range(1, count)
        .Select(i => new ImageDto 
        { 
            ImageId = i,
            Name = $"Random Image {i}",
            UrlImage = $"bilder/test/random{i}",
            CategoryId = 1
        })
        .ToList();
    
    public int GetCountedAllImages() => 100;
    
    public int GetCountedCategoryId(int categoryId) => 10;
    
    public Task<bool> DeleteImageAsync(int id) => Task.FromResult(true);
    
    public List<ImageDto> GetLatestImageList(int limit) => Enumerable.Range(1, limit)
        .Select(i => new ImageDto 
        { 
            ImageId = i,
            Name = $"Latest Image {i}",
            UrlImage = $"bilder/test/latest{i}",
            CategoryId = 1,
            DateUploaded = DateTime.Now.AddDays(-i)
        })
        .ToList();
    
    public ImageDto GetImageById(int id) => GetById(id);
    
    public bool DeleteImage(int id) => DeleteImgId(id);
    
    public Task<IEnumerable<ImageDto>> GetAllImagesAsync() => Task.FromResult<IEnumerable<ImageDto>>(GetAll());
    
    public Task<ImageDto> GetOneImageAsync(int id) => Task.FromResult(GetById(id));
    
    public Task<bool> CreateImageAsync(ImageDto image) => Task.FromResult(AddImage(image));
    
    public Task<bool> UpdateImageAsync(ImageDto image) => Task.FromResult(true);
}
