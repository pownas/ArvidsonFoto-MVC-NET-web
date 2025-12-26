using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.DTOs;
using ArvidsonFoto.Services;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of IImageService for unit testing
/// Uses Core.Models.TblImage from ArvidsonFotoCoreDbSeeder
/// </summary>
public class MockImageService : IImageService
{
    public bool AddImage(TblImage image) => true;
    public TblImage GetById(int id) => new TblImage { ImageId = id, ImageUrlName = "test-image" };
    public TblImage GetOneImageFromCategory(int categoryID) => new TblImage { ImageId = 1, ImageCategoryId = categoryID };
    public bool UpdateImage(UploadImageInputDto image) => true;
    public List<TblImage> GetAll() => [new TblImage { ImageId = 1 }];
    public List<TblImage> GetAllImagesByCategoryID(int categoryID) => [new TblImage { ImageId = 1, ImageCategoryId = categoryID }];
    public bool RemoveImage(int imageID) => true;
}
