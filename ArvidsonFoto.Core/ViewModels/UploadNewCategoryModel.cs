namespace ArvidsonFoto.Core.ViewModels;

/// <summary>
/// ViewModel for creating new categories functionality.
/// </summary>
/// <remarks>
/// Contains the necessary fields for creating a new category including 
/// validation requirements and tracking the creation status.
/// </remarks>
public class UploadNewCategoryModel
{
    /// <summary>Kategorinamn som ska visas i menyn</summary>
    [Required(ErrorMessage = "Ange ett kategorinamn")]
    public required string MenuText { get; set; }

    /// <summary>Huvudmeny ID som den nya kategorin ska kopplas till</summary>
    [Required(ErrorMessage = "Välj en kategori att koppla den nya till")]
    public int? MainMenuId { get; set; }

    /// <summary>Indikerar om kategorin har skapats framgångsrikt</summary>
    public bool CategoryCreated { get; set; }
}
