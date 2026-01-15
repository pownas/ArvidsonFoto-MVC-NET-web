namespace ArvidsonFoto.Core.Interfaces;

/// <summary>
/// Interface för Facebook Graph API-integration
/// </summary>
public interface IFacebookService
{
    /// <summary>
    /// Skapar ett Facebook-inlägg med bilder och text
    /// </summary>
    /// <param name="imageUrls">Lista med fullständiga URL:er till bilder (1-10st)</param>
    /// <param name="message">Texten som ska följa med inlägget</param>
    /// <returns>URL till det skapade Facebook-inlägget, eller null om det misslyckades</returns>
    Task<string?> CreatePostAsync(List<string> imageUrls, string message);

    /// <summary>
    /// Verifierar att Facebook-tjänsten är korrekt konfigurerad
    /// </summary>
    /// <returns>True om konfigurationen är giltig</returns>
    bool IsConfigured();
}
