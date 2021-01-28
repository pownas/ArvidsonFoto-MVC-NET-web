using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Data
{
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
            bool success = false;
            try
            {
                _entityContext.TblImages.Add(image);
                _entityContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                throw new Exception("Fel vid länkning av bild. Felmeddelande: " + ex.Message);
            }
            return success;
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

            if (category.Equals(1))
            {
                image = _entityContext.TblImages
                                      .Where(i => i.ImageArt.Equals(54))
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
}
