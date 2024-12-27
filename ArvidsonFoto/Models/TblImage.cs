#nullable disable
namespace ArvidsonFoto.Models;

/// <summary>
/// Tabellen Image (databas record)
/// </summary>
/// <remarks>
/// Information om en bild i databasen.
/// </remarks>
public partial class TblImage
{
    /// <summary>
    /// Tabellens Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Bildens Id
    /// </summary>
    public int ImageId { get; set; }

    /// <summary>
    /// Bildens huvudfamilj kategori Id (ex. 1 = Fåglar)
    /// </summary>
    public int? ImageHuvudfamilj { get; set; }

    /// <summary>
    /// Bildens familj kategori Id (ex. 2 = Duvor)
    /// </summary>
    public int? ImageFamilj { get; set; }

    /// <summary>
    /// Bildens art kategori Id (ex. 3 = Skogsduva)
    /// </summary>
    public int ImageArt { get; set; }

    /// <summary>
    /// Bildens namn i url (vad bilden ska heta i url)
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    /// Bildens fotograferingsdatum
    /// </summary>
    public DateTime? ImageDate { get; set; }

    /// <summary>
    /// Bildens beskrivning
    /// </summary>
    public string ImageDescription { get; set; }

    /// <summary>
    /// Bildens uppdateringsdatum (när bilden laddades upp)
    /// </summary>
    public DateTime ImageUpdate { get; set; }

    /// <summary>
    /// Extra fält: Ett namn, om det är en kategori som sökts fram.
    /// </summary>
    /// <remarks>
    /// Inte något fält i databasen, men kan sättas ett namn, om det är en kategori som sökts fram.
    /// </remarks>
    [NotMapped]
    public string Name { get; set; }
}