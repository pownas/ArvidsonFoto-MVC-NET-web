using ArvidsonFoto.Core.ApiResponses;
using ArvidsonFoto.Core.DTOs;

namespace ArvidsonFoto.Core.Interfaces;

/// <summary>
/// Interface for category services providing CRUD operations and category management functionality.
/// </summary>
/// <remarks>
/// This interface defines methods for managing categories including creation, retrieval,
/// updating, and deletion operations, as well as specialized methods for menu generation
/// and URL handling.
/// </remarks>
public interface IApiCategoryService
{
    /// <summary>Lägger till en ny kategori</summary>
    /// <param name="category">Kategorin som ska läggas till</param>
    /// <returns>True om kategorin lades till framgångsrikt</returns>
    bool AddCategory(CategoryDto category);

    /// <summary>Hämtar det senaste ID:t för kategorier</summary>
    /// <returns>Det senaste kategori-ID:t</returns>
    int GetLastId();

    /// <summary>Hämtar kategori-ID baserat på kategorinamn</summary>
    /// <param name="categoryName">Namnet på kategorin</param>
    /// <returns>Kategori-ID eller -1 om kategorin inte hittas</returns>
    int GetIdByName(string categoryName);

    /// <summary>Hämtar kategorinamn baserat på kategori-ID</summary>
    /// <param name="id">Kategori-ID</param>
    /// <returns>Kategorinamn eller tom sträng om kategorin inte hittas</returns>
    string GetNameById(int? id);

    /// <summary>Hämtar en kategori baserat på dess ID</summary>
    /// <param name="id">Kategori-ID</param>
    /// <returns>Kategorin eller en standardkategori om den inte hittas</returns>
    public CategoryDto GetById(int? id);

    /// <summary>Hämtar en kategori baserat på dess namn</summary>
    /// <param name="categoryName">Namnet på kategorin</param>
    /// <returns>Kategorin eller en standardkategori om den inte hittas</returns>
    CategoryDto GetByName(string categoryName);

    /// <summary>Hämtar alla kategorier</summary>
    /// <returns>Lista med alla kategorier</returns>
    List<CategoryDto> GetAll();

    /// <summary>Hämtar alla underkategorier för en given föräldrakategori</summary>
    /// <param name="parentCategoryId">Föräldrakategori-ID</param>
    /// <returns>Lista med underkategorier</returns>
    List<CategoryDto> GetChildrenByParentId(int parentCategoryId);

    /// <summary>Hämtar kategori-URL baserat på kategori-ID</summary>
    /// <param name="id">Kategori-ID</param>
    /// <returns>Kategori-URL</returns>
    string GetCategoryUrl(int? id);

    /// <summary>Hämtar huvudmeny-responsen</summary>
    /// <returns>Huvudmeny-respons med alla kategorier</returns>
    MainMenuResponse GetMainMenu();
    
    /// <summary>Räknar alla underkategorier</summary>
    /// <returns>Antal underkategorier</returns>
    int GetAllSubCategoriesCounted();
    
    /// <summary>Hämtar alla kategorier (alias för GetAll)</summary>
    /// <returns>Lista med alla kategorier</returns>
    List<CategoryDto> GetAllCategories();
    
    /// <summary>Hämtar underkategorier för en given föräldrakategori</summary>
    /// <param name="parentId">Föräldrakategori-ID</param>
    /// <returns>Lista med underkategorier</returns>
    List<CategoryDto> GetSubsList(int parentId);
    
    /// <summary>Skapar en ny kategori asynkront</summary>
    /// <param name="category">Kategorin som ska skapas</param>
    /// <returns>True om kategorin skapades framgångsrikt</returns>
    Task<bool> CreateCategoryAsync(CategoryDto category);
    
    /// <summary>Hämtar alla kategorier asynkront</summary>
    /// <returns>Lista med alla kategorier</returns>
    Task<List<CategoryDto>> GetAllCategoriesAsync();
    
    /// <summary>Tar bort en kategori</summary>
    /// <param name="category">Kategorin som ska tas bort</param>
    /// <returns>True om kategorin togs bort framgångsrikt</returns>
    bool DeleteCategory(CategoryDto category);
    /// <summary>
    /// Deletes a category by its ID.
    /// </summary>
    /// <param name="id">The ID of the category to delete.</param>
    /// <returns><see langword="true"/> if the category was successfully deleted; otherwise, <see langword="false"/>.</returns>
    bool DeleteCategory(int? id);
    
    /// <summary>
    /// Updates a category with new information.
    /// </summary>
    /// <param name="updatedCategory">The updated category information.</param>
    /// <returns><see langword="true"/> if the category was successfully updated; otherwise, <see langword="false"/>.</returns>
    bool UpdateCategory(CategoryDto updatedCategory);
    
    /// <summary>
    /// Retrieves a category by its URL segment.
    /// </summary>
    /// <param name="urlSegment">The URL segment of the category to retrieve.</param>
    /// <returns>The category with the specified URL segment, or a default "not found" category if no match is found.</returns>
    CategoryDto GetByUrlSegment(string urlSegment);

    /// <summary>
    /// Gets the ID of a category based on its URL segment.
    /// </summary>
    /// <param name="urlSegment">The URL segment of the category.</param>
    /// <returns>The ID of the category, or -1 if no category with the specified URL segment is found.</returns>
    int GetIdByUrlSegment(string urlSegment);
    
    /// <summary>
    /// Retrieves a category by its URL segment with fallback to numeric ID if the segment is numeric.
    /// </summary>
    /// <param name="urlSegment">The URL segment or potentially a numeric ID of the category to retrieve.</param>
    /// <returns>The category found by URL segment or ID, or a default "not found" category if no match is found.</returns>
    CategoryDto GetByUrlSegmentWithFallback(string urlSegment);
    
    /// <summary>
    /// Gets all main (root) categories with no parent
    /// </summary>
    /// <returns>A list of main categories</returns>
    List<CategoryDto> GetMainCategories();

    /// <summary>
    /// Gets the category path for an image category
    /// </summary>
    /// <param name="categoryId">The category ID to get the path for</param>
    /// <returns>The category path string (e.g., "faglar/tattingar/blames")</returns>
    string GetCategoryPathForImage(int categoryId);

    /// <summary>
    /// Bulk load category paths for multiple category IDs to optimize database queries
    /// </summary>
    /// <param name="categoryIds">List of category IDs to get paths for</param>
    /// <returns>Dictionary mapping category ID to its full path</returns>
    Dictionary<int, string> GetCategoryPathsBulk(List<int> categoryIds);
}