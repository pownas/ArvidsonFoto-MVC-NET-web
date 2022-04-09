using System.Data;

namespace ArvidsonFoto.Services;

public class ImageService : IImageService
{
    // Databas koppling
    private readonly ArvidsonFotoDbContext _entityContext;
    public ImageService(ArvidsonFotoDbContext context)
    {
        _entityContext = context;
    }

    public bool AddImage(TblImage image)
    {
        bool success; //default är false
        try
        {
            _entityContext.TblImages.Add(image);
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

    public bool UpdateImage(UploadImageInputModel updatedImage)
    {
        bool success; //default är false
        try
        {
            TblImage imageToEdit = GetById(updatedImage.ImageId);

            imageToEdit.ImageUrl = updatedImage.ImageUrl; //Filnamn
            imageToEdit.ImageDate = updatedImage.ImageDate; //Fotodatum
            imageToEdit.ImageDescription = updatedImage.ImageDescription; //Beskrivning

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

    public bool DeleteImgId(int imgId)
    {
        bool succeeded = false; //verkar som det måste heta "success" för att defaulta till false. För det går inte att ta bort false tilldelningen.
        try
        {
            TblImage image = _entityContext.TblImages.FirstOrDefault(i => i.ImageId == imgId);
            _entityContext.TblImages.Remove(image);
            _entityContext.SaveChanges();
            succeeded = true;
        }
        catch (Exception ex)
        {
            Log.Error("Error when deleting the image with id: " + imgId + ". Error-message: " + ex.Message);
        }
        return succeeded;
    }

    public int GetImageLastId()
    {
        int highestID = -1;
        highestID = _entityContext.TblImages.OrderBy(i => i.ImageId).LastOrDefault().ImageId;
        return highestID;
    }

    public TblImage GetOneImageFromCategory(int category)
    {
        TblImage image;

        if (category.Equals(1)) //Om man söker fram Id = 1 (Fåglar) , så ska Id för Blåmes hittas och visas bilden för istället. 
        {
            int blamesId = _entityContext.TblMenus
                                         .Where(m => m.MenuText.Equals("Blåmes"))
                                         .FirstOrDefault()
                                         .MenuId;

            image = _entityContext.TblImages
                                  .Where(i => i.ImageArt.Equals(blamesId))
                                  .OrderByDescending(i => i.ImageUpdate)
                                  .FirstOrDefault();
        }
        else
        {
            image = _entityContext.TblImages
                                  .Where(i => i.ImageArt.Equals(category)
                                           || i.ImageFamilj.Equals(category)
                                           || i.ImageHuvudfamilj.Equals(category))
                                  .OrderByDescending(i => i.ImageUpdate)
                                  .FirstOrDefault();
        }
        return image;
    }

    public List<TblImage> GetAll()
    {
        List<TblImage> images = _entityContext.TblImages.ToList();
        return images;
    }

    /// <summary> Används på startsidan för random antal av bilder. </summary>
    /// <param name="count">Antal bilder som ska plockas ifrån databasen</param>
    /// <returns>"count" antal bilder ifrån databasen</returns>
    public List<TblImage> GetRandomNumberOfImages(int count)
    {
        List<TblImage> images = _entityContext.TblImages
                               .OrderBy(r => Guid.NewGuid()) //Här gör jag en random med hjälp av en ny GUID som random nummer.
                               .Take(count)
                               .ToList();
        return images;
    }

    public List<TblImage> GetAllImagesByCategoryID(int categoryID)
    {
        List<TblImage> images = _entityContext.Set<TblImage>()
                               .Where(i => i.ImageArt == categoryID
                                        || i.ImageFamilj == categoryID
                                        || i.ImageHuvudfamilj == categoryID)
                               .ToList();
        return images;
    }

    public TblImage GetById(int imageId)
    {
        TblImage image = _entityContext.TblImages
                              .Where(i => i.ImageId.Equals(imageId))
                              .FirstOrDefault();
        return image;
    }

}