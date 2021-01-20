using System;
using System.Collections.Generic;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Data
{
    public interface IImageService
    {
        bool SetImageInsert(TblImage image);
        
        int GetImageLastId();
        
        List<TblImage> GetAllImagesList();

        List<TblImage> GetRandomNumberOfImages(int count);

        List<TblImage> GetAllImagesByCategoryID(int categoryID);
    }
}