using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using ArvidsonFoto.Services;

namespace ArvidsonFoto.Tests.Unit.MockServices;

public class MockImageService : IImageService
{
    public bool AddImage(TblImage image) => true;
    public bool UpdateImage(UploadImageInputModel image) => true;
    public bool DeleteImgId(int imgId) => true;

    public int GetImageLastId() => 
        DbSeederExtension.DbSeed_Tbl_Image.Max(x => x.Id);

    public TblImage GetById(int imageId) =>
        DbSeederExtension.DbSeed_Tbl_Image
            .Where(i => i.ImageId.Equals(imageId))
            .FirstOrDefault() ?? new TblImage();

    public TblImage GetOneImageFromCategory(int category) =>
         DbSeederExtension.DbSeed_Tbl_Image
            .Where(i => i.ImageArt.Equals(category)
                     || i.ImageFamilj.Equals(category)
                     || i.ImageHuvudfamilj.Equals(category))
            .OrderByDescending(i => i.ImageUpdate)
            .FirstOrDefault() ?? DbSeederExtension.DbSeed_Tbl_Image
            .Where(i => i.ImageArt.Equals(13)) // 13 == Blåmes
            .OrderByDescending(i => i.ImageUpdate)
            .First();

    public List<TblImage> GetAll() =>
        DbSeederExtension.DbSeed_Tbl_Image ?? new List<TblImage>();

    public List<TblImage> GetRandomNumberOfImages(int count) =>
        DbSeederExtension.DbSeed_Tbl_Image
            .OrderBy(r => Guid.NewGuid())
            .Take(count)
            .ToList() ?? new List<TblImage>();

    public List<TblImage> GetAllImagesByCategoryID(int categoryID) =>
        DbSeederExtension.DbSeed_Tbl_Image
            .Where(i => i.ImageArt == categoryID
                     || i.ImageFamilj == categoryID
                     || i.ImageHuvudfamilj == categoryID)
            .ToList() ?? new List<TblImage>();
}
