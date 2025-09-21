using ArvidsonFoto.Core.DTOs;

namespace ArvidsonFoto.Core.ApiResponses;

/// <summary>
/// Represents the response for the main menu, including its categories and metadata about subcategories.
/// </summary>
/// <remarks>This class provides information about the main menu structure, including whether subcategories exist
/// and the total count of subcategories. It is typically used to convey menu-related data in applications that support
/// hierarchical menu structures.</remarks>
public class MainMenuResponse()
{
    /// <summary> Huvudmenyns kategorier </summary>
    public List<MainMenuDto> MainMenu { get; set; } = new();

    /// <summary>
    /// Determines whether the specified key exists in the menu collection.
    /// </summary>
    /// <param name="key">The key to locate in the menu collection. Typically represents a menu URL.</param>
    /// <returns><see langword="true"/> if the specified key exists in the menu collection; otherwise, <see langword="false"/>.</returns>
    public bool ContainsKey(string key)
    {
        return MainMenu.Any(x => x.MenuUrl == key);
    }
}
