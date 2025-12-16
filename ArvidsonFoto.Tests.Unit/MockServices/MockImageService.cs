using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of IImageService for unit testing
/// Uses data from ArvidsonFotoCoreDbSeeder to match production data structure
/// </summary>
public class MockImageService : IImageService
{
    public bool AddImage(TblImage image) => true;
    public bool UpdateImage(UploadImageInputModel image) => true;
    public bool DeleteImgId(int imgId) => true;

    public int GetImageLastId() => 
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image.Max(x => x.Id ?? 0);

    public TblImage GetById(int imageId)
    {
        var coreImage = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image
            .FirstOrDefault(i => i.ImageId == imageId);
        
        if (coreImage == null)
            return new TblImage();
        
        return MapToOldModel(coreImage);
    }

    public TblImage GetOneImageFromCategory(int category)
    {
        var coreImage = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image
            .Where(i => i.ImageCategoryId == category
                     || i.ImageFamilyId == category
                     || i.ImageMainFamilyId == category)
            .OrderByDescending(i => i.ImageUpdate)
            .FirstOrDefault();

        if (coreImage != null)
            return MapToOldModel(coreImage);

        // Fallback till Blåmes (MenuCategoryId=243 i nya datan)
        coreImage = ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image
            .Where(i => i.ImageCategoryId == 243) // 243 == Blåmes i nya datan
            .OrderByDescending(i => i.ImageUpdate)
            .FirstOrDefault();

        return coreImage != null ? MapToOldModel(coreImage) : new TblImage();
    }

    public List<TblImage> GetAll() =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image
            .Select(MapToOldModel)
            .ToList();

    public List<TblImage> GetRandomNumberOfImages(int count) =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image
            .OrderBy(r => Guid.NewGuid())
            .Take(count)
            .Select(MapToOldModel)
            .ToList();

    public List<TblImage> GetAllImagesByCategoryID(int categoryID) =>
        ArvidsonFotoCoreDbSeeder.DbSeed_Tbl_Image
            .Where(i => i.ImageCategoryId == categoryID
                     || i.ImageFamilyId == categoryID
                     || i.ImageMainFamilyId == categoryID)
            .Select(MapToOldModel)
            .ToList();

    /// <summary>
    /// Maps from Core.Models.TblImage to Models.TblImage (old model)
    /// </summary>
    private static TblImage MapToOldModel(Core.Models.TblImage coreImage)
    {
        return new TblImage
        {
            Id = coreImage.Id ?? 0,
            ImageId = coreImage.ImageId ?? 0,
            ImageHuvudfamilj = coreImage.ImageMainFamilyId,
            ImageFamilj = coreImage.ImageFamilyId,
            ImageArt = coreImage.ImageCategoryId ?? 0,
            ImageUrl = coreImage.ImageUrlName ?? string.Empty,
            ImageDate = coreImage.ImageDate,
            ImageDescription = coreImage.ImageDescription ?? string.Empty,
            ImageUpdate = coreImage.ImageUpdate ?? DateTime.Now,
            Name = coreImage.Name ?? string.Empty
        };
    }
}
