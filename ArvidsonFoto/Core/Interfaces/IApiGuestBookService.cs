
using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Core.Interfaces;

/// <summary>
/// Interface for guestbook services providing CRUD operations for guestbook entries.
/// </summary>
/// <remarks>
/// This interface defines methods for managing guestbook entries including creation,
/// retrieval, deletion, and status management operations.
/// </remarks>
public interface IApiGuestBookService
{
    /// <summary>Skapar ett nytt gästboksinlägg</summary>
    /// <param name="gb">Gästboksinlägget som ska skapas</param>
    /// <returns>True om inlägget skapades framgångsrikt</returns>
    bool CreateGBpost(TblGb gb);

    /// <summary>Markerar ett gästboksinlägg som läst</summary>
    /// <param name="gbId">ID för gästboksinlägget</param>
    /// <returns>True om inlägget markerades som läst framgångsrikt</returns>
    bool ReadGbPost(int gbId);

    /// <summary>Tar bort ett gästboksinlägg</summary>
    /// <param name="gbId">ID för gästboksinlägget som ska tas bort</param>
    /// <returns>True om inlägget togs bort framgångsrikt</returns>
    bool DeleteGbPost(int gbId);

    /// <summary>Hämtar antal olästa gästboksinlägg</summary>
    /// <returns>Antal olästa inlägg</returns>
    int GetCountOfUnreadPosts();

    /// <summary>Hämtar det senaste gästboks-ID:t</summary>
    /// <returns>Det senaste gästboks-ID:t</returns>
    int GetLastGbId();

    /// <summary>Hämtar alla gästboksinlägg</summary>
    /// <returns>Lista med alla gästboksinlägg</returns>
    List<TblGb> GetAll();
    
    /// <summary>Hämtar alla gästboksinlägg asynkront</summary>
    /// <returns>Lista med alla gästboksinlägg</returns>
    Task<IEnumerable<TblGb>> GetAllGuestbookEntriesAsync();
    
    /// <summary>Hämtar ett specifikt gästboksinlägg asynkront</summary>
    /// <param name="id">ID för gästboksinlägget</param>
    /// <returns>Det begärda gästboksinlägget</returns>
    Task<TblGb> GetOneGbEntryAsync(int id);
    
    /// <summary>Skapar ett nytt gästboksinlägg asynkront</summary>
    /// <param name="gb">Gästboksinlägget som ska skapas</param>
    /// <returns>True om inlägget skapades framgångsrikt</returns>
    Task<bool> CreateGbEntryAsync(TblGb gb);
    
    /// <summary>Tar bort ett gästboksinlägg asynkront</summary>
    /// <param name="id">ID för gästboksinlägget som ska tas bort</param>
    /// <returns>True om inlägget togs bort framgångsrikt</returns>
    Task<bool> DeleteGbEntryAsync(int id);
    
    /// <summary>Räknar alla gästboksinlägg</summary>
    /// <returns>Totalt antal gästboksinlägg</returns>
    int GetAllGuestbookEntriesCounted();
}