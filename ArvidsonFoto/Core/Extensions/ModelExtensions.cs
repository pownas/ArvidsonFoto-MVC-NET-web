using ArvidsonFoto.Core.Models;
using System.Globalization;

namespace ArvidsonFoto.Core.Extensions;

/// <summary>
/// Extension methods to provide backward compatibility with old property names
/// </summary>
public static class ModelExtensions
{
    #region TblMenu Extensions

    /// <summary>
    /// Gets the MenuId (maps to MenuCategoryId in the new model)
    /// </summary>
    public static int MenuId(this TblMenu menu) => menu.MenuCategoryId ?? 0;

    /// <summary>
    /// Gets the MenuText (maps to MenuDisplayName in the new model)
    /// </summary>
    public static string MenuText(this TblMenu menu) => menu.MenuDisplayName ?? string.Empty;

    /// <summary>
    /// Gets the MenuMainId (maps to MenuParentCategoryId in the new model)
    /// </summary>
    public static int? MenuMainId(this TblMenu menu) => menu.MenuParentCategoryId;

    #endregion

    #region TblImage Extensions

    /// <summary>
    /// Gets the ImageName (maps to ImageUrlName in the new model)
    /// </summary>
    public static string ImageName(this TblImage image) => image.ImageUrlName ?? string.Empty;

    /// <summary>
    /// Gets the ImageArt (maps to ImageCategoryId in the new model)
    /// </summary>
    public static int ImageArt(this TblImage image) => image.ImageCategoryId ?? 0;

    /// <summary>
    /// Gets the ImageHuvudfamilj (maps to ImageMainFamilyId in the new model)
    /// </summary>
    public static int? ImageHuvudfamilj(this TblImage image) => image.ImageMainFamilyId;

    /// <summary>
    /// Gets the ImageFamilj (maps to ImageFamilyId in the new model)
    /// </summary>
    public static int? ImageFamilj(this TblImage image) => image.ImageFamilyId;

    /// <summary>
    /// Gets the ImageDate safely
    /// </summary>
    public static DateTime GetImageDate(this TblImage image) => 
        image.ImageDate ?? DateTime.Now;

    #endregion

    #region TblGb Extensions

    /// <summary>
    /// Gets the GbMessage (maps to GbText in the new model)
    /// </summary>
    public static string GbMessage(this TblGb gb) => gb.GbText;

    /// <summary>
    /// Gets the GbDate safely
    /// </summary>
    public static DateTime GetGbDate(this TblGb gb) => 
        gb.GbDate ?? DateTime.Now;

    #endregion

    #region DateTime Extensions
    
    /// <summary>
    /// Extension to allow ToString with format parameter
    /// </summary>
    public static string ToString(this DateTime dateTime, string format) => 
        dateTime.ToString(format, CultureInfo.InvariantCulture);

    #endregion
}
