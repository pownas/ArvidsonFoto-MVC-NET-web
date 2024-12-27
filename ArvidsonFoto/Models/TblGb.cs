#nullable disable
namespace ArvidsonFoto.Models;

/// <summary>
/// Tabellen Gb - GästBoken (databas record)
/// </summary>
public partial class TblGb()
{
    /// <summary>
    /// Tabellens databas Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gästboksinläggets Id
    /// </summary>
    public int GbId { get; set; }

    /// <summary>
    /// Gästboksinläggets titel/rubrik
    /// </summary>
    public string GbName { get; set; }

    /// <summary>
    /// Inläggets avsändares e-postadress
    /// </summary>
    public string GbEmail { get; set; }

    /// <summary>
    /// Inläggets avsändares hemsida
    /// </summary>
    public string GbHomepage { get; set; }

    /// <summary>
    /// Inläggets text
    /// </summary>
    public string GbText { get; set; }

    /// <summary>
    /// Inläggets datum
    /// </summary>
    public DateTime? GbDate { get; set; }

    /// <summary>
    /// Har inlägget markerats som läst?
    /// </summary>
    public bool? GbReadPost { get; set; }

    /// <summary>
    /// Inläggets avsändares IP-adress
    /// </summary>
    public string GbIp { get; set; }
}