using ArvidsonFoto.Models;
using System.Collections.Generic;

namespace ArvidsonFoto.Services
{
    public interface IImageService
    {
        public bool AddImage(TblImage image);

        public bool UpdateImage(UploadImageInputModel image);

        bool DeleteImgId(int imgId);

        int GetImageLastId();

        TblImage GetById(int imageId);

        TblImage GetOneImageFromCategory(int category);

        List<TblImage> GetAll();

        List<TblImage> GetRandomNumberOfImages(int count);

        List<TblImage> GetAllImagesByCategoryID(int categoryID);
    }
}