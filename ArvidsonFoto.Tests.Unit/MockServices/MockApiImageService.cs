using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of IApiImageService for unit testing
/// Uses in-memory test data without external dependencies
/// </summary>
public class MockApiImageService : IApiImageService
{
    private readonly List<ImageDto> _testImages;

    public MockApiImageService()
    {
        // Create a fresh copy for each instance to avoid test interference
        _testImages = new List<ImageDto>
        {
            new ImageDto
            {
                ImageId = 1,
                CategoryId = 11,
                Name = "Fjällabb",
                UrlImage = "08TA3696",
                UrlCategory = "faglar/alkor-och-labbar/fjallabb",
                DateImageTaken = new DateTime(2021, 11, 22, 16, 21, 00),
                DateUploaded = new DateTime(2021, 11, 22, 16, 21, 00),
                Description = "En fjällabbs beskrivning..."
            },
            new ImageDto
            {
                ImageId = 2,
                CategoryId = 13,
                Name = "Blåmes",
                UrlImage = "B57W4725",
                UrlCategory = "faglar/tattingar/mesar/blames",
                DateImageTaken = new DateTime(2021, 11, 22, 16, 21, 00),
                DateUploaded = new DateTime(2021, 11, 22, 16, 21, 00),
                Description = "Hane, beskrivning av blåmes...."
            },
            new ImageDto
            {
                ImageId = 3,
                CategoryId = 14,
                Name = "Björn",
                UrlImage = "B59W4837",
                UrlCategory = "daggdjur/bjorn",
                DateImageTaken = new DateTime(2021, 11, 22, 16, 21, 00),
                DateUploaded = new DateTime(2021, 11, 22, 16, 21, 00),
                Description = "i Sverige"
            },
            new ImageDto
            {
                ImageId = 4,
                CategoryId = 15,
                Name = "Hasselsnok",
                UrlImage = "13TA5142",
                UrlCategory = "kraldjur/hasselsnok",
                DateImageTaken = new DateTime(2021, 11, 22, 16, 21, 00),
                DateUploaded = new DateTime(2021, 11, 22, 16, 21, 00),
                Description = ""
            },
            new ImageDto
            {
                ImageId = 5,
                CategoryId = 16,
                Name = "Ekoxe",
                UrlImage = "B60W1277",
                UrlCategory = "insekter/skalbaggar",
                DateImageTaken = new DateTime(2021, 11, 22, 16, 21, 00),
                DateUploaded = new DateTime(2021, 11, 22, 16, 21, 00),
                Description = "Ekoxe, hane"
            }
        };
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
        if (categoryId == 1) // Special case for main category "Fåglar"
        {
            // Return Blåmes image
            categoryId = 13;
        }
        
        var image = _testImages
            .Where(i => i.CategoryId == categoryId)
            .OrderByDescending(i => i.DateUploaded)
            .FirstOrDefault();
            
        return image ?? CreateNotFoundImage();
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