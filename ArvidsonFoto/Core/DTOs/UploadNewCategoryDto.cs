using System.ComponentModel.DataAnnotations;

namespace ArvidsonFoto.Core.DTOs;

/// <summary>
/// Data Transfer Object (DTO) for uploading new categories.
/// </summary>
/// <remarks>
/// Contains the necessary fields for creating a new category including 
/// validation requirements and tracking the creation status.
/// </remarks>
public class UploadNewCategoryDto
{
    /// <summary>Kategorinamn som ska visas i menyn</summary>
    [Required(ErrorMessage = "Ange ett kategorinamn")]
    public required string MenuText { get; set; }

    /// <summary>Huvudmeny ID som den nya kategorin ska kopplas till</summary>
    [Required(ErrorMessage = "Välj en kategori att koppla den nya till")]
    public int? MainMenuId { get; set; }

    /// <summary>Indikerar om kategorin har skapats framgångsrikt</summary>
    public bool CategoryCreated { get; set; }

    /// <summary>
    /// Skapar en tom UploadNewCategoryDto med alla required fields initialiserade
    /// </summary>
    /// <returns>En ny tom UploadNewCategoryDto</returns>
    public static UploadNewCategoryDto CreateEmpty()
    {
        return new UploadNewCategoryDto
        {
            MenuText = string.Empty,
            MainMenuId = null,
            CategoryCreated = false
        };
    }
}