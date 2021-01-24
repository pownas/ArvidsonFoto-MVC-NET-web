using System;
using System.Collections.Generic;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Data
{
    public interface IImageService
    {
        bool SetImageInsert(TblImage image);
        
        int GetImageLastId();

        TblImage GetById(int imageId);

        TblImage GetOneImageFromCategory(int category);

        List<TblImage> GetAll();

        List<TblImage> GetRandomNumberOfImages(int count);

        List<TblImage> GetAllImagesByCategoryID(int categoryID);
    }
}