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

        public bool SetImageInsert(TblImage image)
        {
            throw new NotImplementedException();
        }

        public int GetImageLastId()
        {
            throw new NotImplementedException();
        }

        public TblImage GetOneImageFromCategory(int category)
        {
            TblImage image;
            image = _entityContext.TblImages.Where(i => i.ImageArt.Equals(category) || i.ImageFamilj.Equals(category) || i.ImageHuvudfamilj.Equals(category)).OrderByDescending(i => i.ImageUpdate).FirstOrDefault();
            return image;
        }

        public List<TblImage> GetAll()
        {
            List<TblImage> images;
            images = _entityContext.TblImages.ToList();
            return images;
        }

        /// <summary> Används på startsidan för random antal av bilder. </summary>
        /// <param name="count">Antal bilder som ska plockas ifrån databasen</param>
        /// <returns>"count" antal bilder ifrån databasen</returns>
        public List<TblImage> GetRandomNumberOfImages(int count)
        {
            List<TblImage> images;
            images = _entityContext.TblImages
                                   .OrderBy(r => Guid.NewGuid()) //Här gör jag en random med hjälp av en ny GUID som random nummer.
                                   .Take(count)
                                   .ToList();
            return images;
        }

        public List<TblImage> GetAllImagesByCategoryID(int categoryID)
        {
            List<TblImage> images;
            images = _entityContext.Set<TblImage>().Where(i => i.ImageArt == categoryID || i.ImageFamilj == categoryID || i.ImageHuvudfamilj == categoryID).ToList();
            //if (images is null) //Om ImageHuvudfamilj ger null, kolla mot Fåglar....
            //{
            //    images = _entityContext.TblImages.Where(i => i.ImageHuvudfamilj == categoryID).ToList();
            //}
            return images;
        }

        // Databas koppling
        //private readonly SqlConnectionConfiguration _configuration;
        //public ImageService(SqlConnectionConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        //public async Task<bool> SetImageInsert(Image image)
        //{
        //    using (var conn = new SqlConnection(_configuration.Value))
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("image_ID", image.image_ID, DbType.Int32);
        //        parameters.Add("image_art", image.image_art, DbType.Int32);
        //        parameters.Add("image_familj", image.image_familj, DbType.Int32);
        //        parameters.Add("image_huvudfamilj", image.image_huvudfamilj, DbType.Int32);
        //        parameters.Add("image_description", image.image_description, DbType.String);
        //        parameters.Add("image_URL", image.image_URL, DbType.String);
        //        parameters.Add("image_date", image.image_date, DbType.DateTime);
        //        parameters.Add("image_date", image.image_update, DbType.DateTime);

        //        // Raw SQL method
        //        const string query = @"INSERT INTO tbl_images (image_ID, image_art, image_familj, image_huvudfamilj, image_description, image_URL, image_date, image_update) VALUES (@image_ID, @image_art, @image_familj, @image_huvudfamilj, @image_description, @image_URL, @image_date, @image_update)";
        //        await conn.ExecuteAsync(query, new { image.image_ID, image.image_art, image.image_familj, image.image_huvudfamilj, image.image_description, image.image_URL, image.image_date, image.image_update }, commandType: CommandType.Text);
        //    }
        //    return true;
        //}

        //public async Task<int> GetImageLastId()
        //{
        //    int highestID = -1;
        //    using (var conn = new SqlConnection(_configuration.Value))
        //    {
        //        var sql = @"SELECT MAX(image_ID) FROM tbl_images";
        //        highestID = await conn.QuerySingleAsync<int>(sql);
        //    }
        //    return highestID;
        //}


        //public async Task<IEnumerable<Image>> GetAllImagesList()
        //{
        //    IEnumerable<Image> images;
        //    using (var conn = new SqlConnection(_configuration.Value))
        //    {
        //        images = await conn.QueryAsync<Image>(@"SELECT * FROM tbl_images ORDER BY image_ID DESC");
        //    }
        //    return images;
        //}

        //public async Task<IEnumerable<Image>> GetAllImagesByCategoryID(int categoryID)
        //{
        //    IEnumerable<Image> images;
        //    using (var conn = new SqlConnection(_configuration.Value))
        //    {
        //        var art = await conn.QuerySingleOrDefaultAsync<string>(@"SELECT DISTINCT image_art FROM tbl_images WHERE image_art = " + categoryID + "");
        //        var familj = await conn.QuerySingleOrDefaultAsync<string>(@"SELECT DISTINCT image_familj FROM tbl_images WHERE image_familj = " + categoryID + "");
        //        var huvudfamilj = await conn.QuerySingleOrDefaultAsync<string>(@"SELECT DISTINCT image_huvudfamilj FROM tbl_images WHERE image_huvudfamilj = " + categoryID + "");

        //        if (art is not null) {
        //            images = await conn.QueryAsync<Image>(@"SELECT * FROM tbl_images WHERE image_art = " + categoryID + " ORDER BY image_date, image_update DESC");
        //        } else if (familj is not null)
        //        {
        //            images = await conn.QueryAsync<Image>(@"SELECT * FROM tbl_images WHERE image_familj = " + categoryID + " ORDER BY image_date, image_update DESC");
        //        }else if (huvudfamilj is not null)
        //        {
        //            images = await conn.QueryAsync<Image>(@"SELECT * FROM tbl_images WHERE image_huvudfamilj = " + categoryID + " ORDER BY image_date, image_update DESC");
        //        } else
        //        {
        //            images = await conn.QueryAsync<Image>(@"SELECT * FROM tbl_images WHERE image_art = 439 ORDER BY image_date, image_update DESC");
        //        }
        //    }
        //    return images;
        //}

        //public async Task<IEnumerable<Image>> GetAllImagesByArtList(int imageArtID)
        //{
        //    IEnumerable<Image> images;
        //    using (var conn = new SqlConnection(_configuration.Value))
        //    {
        //        images = await conn.QueryAsync<Image>(@"SELECT * FROM tbl_images WHERE image_art = " + imageArtID + " ORDER BY image_date DESC");
        //    }
        //    return images;
        //}

        //public async Task<IEnumerable<Image>> GetAllImagesByArtList(int imageArtID)
        //{
        //    IEnumerable<Image> images;
        //    using (var conn = new SqlConnection(_configuration.Value))
        //    {
        //        images = await conn.QueryAsync<Image>(@"SELECT * FROM tbl_images WHERE image_art = " + imageArtID + " ORDER BY image_date DESC");
        //    }
        //    return images;
        //}
    }
}
