using ArvidsonFoto.Core.DTOs;

namespace ArvidsonFoto.Core.Interfaces;

/// <summary>
/// Interface for image services
/// </summary>
public interface IApiImageService
{
    /// <summary>
    /// Adds a new image to the database
    /// </summary>
    /// <param name="image">The image to add</param>
    /// <returns>True if the image was added successfully</returns>
    public bool AddImage(ImageDto image);

    /// <summary>
    /// Deletes an image by its ID
    /// </summary>
    /// <param name="imgId">The ID of the image to delete</param>
    /// <returns>True if the image was deleted successfully</returns>
    bool DeleteImgId(int imgId);

    /// <summary>
    /// Gets the last image ID
    /// </summary>
    /// <returns>The last image ID</returns>
    int GetImageLastId();

    /// <summary>
    /// Gets an image by its ID
    /// </summary>
    /// <param name="imageId">The ID of the image to get</param>
    /// <returns>The image</returns>
    ImageDto GetById(int imageId);

    /// <summary>
    /// Gets one image from a category
    /// </summary>
    /// <param name="categoryId">The category ID</param>
    /// <returns>An image from the category</returns>
    ImageDto GetOneImageFromCategory(int categoryId);

    /// <summary>
    /// Gets all images
    /// </summary>
    /// <returns>A list of all images</returns>
    List<ImageDto> GetAll();

    /// <summary>
    /// Gets a random number of images
    /// </summary>
    /// <param name="count">The number of images to get</param>
    /// <returns>A list of random images</returns>
    List<ImageDto> GetRandomNumberOfImages(int count);

    /// <summary>
    /// Gets all images by category ID
    /// </summary>
    /// <param name="categoryID">The category ID</param>
    /// <returns>A list of images in the category</returns>
    List<ImageDto> GetImagesByCategoryID(int categoryID);

    /// <summary>Räknar alla bilder</summary>
    /// <returns>Totalt antal bilder</returns>
    int GetCountedAllImages();

    /// <summary>Räknar alla bilder i en kategori</summary>
    /// <returns>En int med kategorins bilder, samt en int med kategorins bilder + alla dess childrens</returns>
    int GetCountedCategoryId(int categoryId);

    /// <summary>Tar bort en bild asynkront</summary>
    /// <param name="id">ID för bilden som ska tas bort</param>
    /// <returns>True om bilden togs bort framgångsrikt</returns>
    Task<bool> DeleteImageAsync(int id);
    
    /// <summary>Hämtar de senaste bilderna</summary>
    /// <param name="limit">Antal bilder att hämta</param>
    /// <returns>Lista med de senaste bilderna</returns>
    List<ImageDto> GetLatestImageList(int limit);
    
    /// <summary>Hämtar en bild baserat på dess ID</summary>
    /// <param name="id">Bild-ID</param>
    /// <returns>Bilden eller en standardbild om den inte hittas</returns>
    ImageDto GetImageById(int id);
    
    /// <summary>Tar bort en bild</summary>
    /// <param name="id">ID för bilden som ska tas bort</param>
    /// <returns>True om bilden togs bort framgångsrikt</returns>
    bool DeleteImage(int id);
    
    /// <summary>Hämtar alla bilder asynkront</summary>
    /// <returns>Lista med alla bilder</returns>
    Task<IEnumerable<ImageDto>> GetAllImagesAsync();
    
    /// <summary>Hämtar en bild asynkront</summary>
    /// <param name="id">Bild-ID</param>
    /// <returns>Den begärda bilden</returns>
    Task<ImageDto> GetOneImageAsync(int id);
    
    /// <summary>Skapar en ny bild asynkront</summary>
    /// <param name="image">Bilden som ska skapas</param>
    /// <returns>True om bilden skapades framgångsrikt</returns>
    Task<bool> CreateImageAsync(ImageDto image);

    /// <summary>
    /// Updates an existing image
    /// </summary>
    /// <param name="image">The image data to update</param>
    /// <returns>True if the image was updated successfully</returns>
    Task<bool> UpdateImageAsync(ImageDto image);
}