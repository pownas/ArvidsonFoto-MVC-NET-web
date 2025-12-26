using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.DTOs;

namespace ArvidsonFoto.Services;

/// <summary>
/// Legacy image service - migrated to use Core namespace
/// </summary>
/// <remarks>
/// Consider migrating to ApiImageService for new functionality
/// </remarks>
public class ImageService : IImageService
{
    // Databas koppling - uppdaterad till Core.Data
    private readonly ArvidsonFotoCoreDbContext _entityContext;

    public ImageService(ArvidsonFotoCoreDbContext context)
    {
        _entityContext = context;
    }

    public TblImage GetById(int id)
    {
        TblImage? image = _entityContext.TblImages.FirstOrDefault(i => i.ImageId.Equals(id));
        if (image is null)
        {
            Log.Warning("Could not find imageId: " + id);
            return new TblImage();
        }
        return image;
    }

    public TblImage GetOneImageFromCategory(int categoryID)
    {
        TblImage? image = _entityContext.TblImages.FirstOrDefault(i => i.ImageCategoryId.Equals(categoryID));
        if (image is null)
        {
            Log.Warning("Could not find an image in categoryID: " + categoryID);
            return new TblImage();
        }
        return image;
    }

    public bool UpdateImage(UploadImageInputDto updatedImage)
    {
        bool success = false;
        try
        {
            TblImage? imgToUpdate = _entityContext.TblImages.FirstOrDefault(i => i.ImageId.Equals(updatedImage.ImageId));

            if (imgToUpdate is not null)
            {
                imgToUpdate.ImageUrlName = updatedImage.ImageUrl;
                imgToUpdate.ImageDate = updatedImage.ImageDate;
                imgToUpdate.ImageDescription = updatedImage.ImageDescription;
                imgToUpdate.ImageUpdate = DateTime.Now;

                _entityContext.SaveChangesAsync().Wait();
                success = true;
            }
        }
        catch (Exception ex)
        {
            success = false;
            throw new Exception("Fel vid uppdatering av bild. Felmeddelande: " + ex.Message);
        }
        return success;
    }

    public List<TblImage> GetAll()
    {
        return _entityContext.TblImages.ToList();
    }

    public List<TblImage> GetAllImagesByCategoryID(int categoryID)
    {
        return _entityContext.TblImages.Where(i => i.ImageCategoryId.Equals(categoryID)).ToList();
    }

    public bool AddImage(TblImage image)
    {
        bool success = false;
        try
        {
            _entityContext.TblImages.Add(image);
            _entityContext.SaveChangesAsync().Wait();
            success = true;
        }
        catch (Exception ex)
        {
            success = false;
            throw new Exception("Fel vid skapande av bild. Felmeddelande: " + ex.Message);
        }
        return success;
    }

    public bool RemoveImage(int imageID)
    {
        bool success = false;
        try
        {
            TblImage? imgToRemove = _entityContext.TblImages.FirstOrDefault(i => i.ImageId.Equals(imageID));
            if (imgToRemove is not null)
            {
                _entityContext.TblImages.Remove(imgToRemove);
                _entityContext.SaveChangesAsync().Wait();
                success = true;
            }
        }
        catch (Exception ex)
        {
            success = false;
            throw new Exception("Fel vid raderade av bild. Felmeddelande: " + ex.Message);
        }
        return success;
    }
}