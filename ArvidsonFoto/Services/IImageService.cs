using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.DTOs;

namespace ArvidsonFoto.Services;

/// <summary>
/// Legacy image service interface - migrated to use Core namespace
/// </summary>
/// <remarks>
/// Consider migrating to ApiImageService for new functionality
/// </remarks>
public interface IImageService
{
    TblImage GetById(int id);
    TblImage GetOneImageFromCategory(int categoryID);
    List<TblImage> GetAll();
    List<TblImage> GetAllImagesByCategoryID(int categoryID);
    bool AddImage(TblImage image);
    bool UpdateImage(UploadImageInputDto image);
    bool RemoveImage(int imageID);
}