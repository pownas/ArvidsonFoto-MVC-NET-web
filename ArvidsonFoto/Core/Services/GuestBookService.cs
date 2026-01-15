using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Interfaces;

namespace ArvidsonFoto.Core.Services;

/// <summary>
/// Guestbook service - manages guestbook entries
/// </summary>
/// <remarks>
/// This service implements guestbook functionality using the Core database context
/// and Core models. It provides methods for creating, reading, updating, and deleting
/// guestbook entries, both synchronously and asynchronously.
/// </remarks>
public class GuestBookService : IGuestBookService
{
    private readonly ArvidsonFotoCoreDbContext _entityContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuestBookService"/> class.
    /// </summary>
    /// <param name="context">The Core database context</param>
    public GuestBookService(ArvidsonFotoCoreDbContext context)
    {
        _entityContext = context;
    }

    public List<TblGb> GetAll()
    {
        return _entityContext.TblGbs.OrderByDescending(g => g.GbId).ToList();
    }

    public bool CreateGBpost(TblGb gb)
    {
        try
        {
            _entityContext.TblGbs.Add(gb);
            _entityContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"Error when creating GB-post. Error-message: {ex.Message}");
            return false;
        }
    }

    public bool ReadGbPost(int gbId)
    {
        try
        {
            TblGb? gb = _entityContext.TblGbs.FirstOrDefault(gb => gb.GbId == gbId);
            if (gb != null)
            {
                gb.GbReadPost = true;
                _entityContext.SaveChanges();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Log.Error($"Error when trying to mark GB-post with id: {gbId}, as read. Error-message: {ex.Message}");
            return false;
        }
    }

    public bool DeleteGbPost(int gbId)
    {
        try
        {
            TblGb? gb = _entityContext.TblGbs.FirstOrDefault(gb => gb.GbId == gbId);
            if (gb != null)
            {
                _entityContext.TblGbs.Remove(gb);
                _entityContext.SaveChanges();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Log.Error($"Error when deleting GB-post with id: {gbId}. Error-message: {ex.Message}");
            return false;
        }
    }

    public int GetCountOfUnreadPosts()
    {
        try
        {
            return _entityContext.TblGbs.Count(gb => gb.GbReadPost == null || gb.GbReadPost == false);
        }
        catch (Exception ex)
        {
            Log.Error($"Error when counting Unread GB-posts. Error-message: {ex.Message}");
            return 0;
        }
    }

    public int GetLastGbId()
    {
        try
        {
            var maxId = _entityContext.TblGbs.Max(gb => (int?)gb.GbId);
            return maxId ?? -1;
        }
        catch
        {
            return -1;
        }
    }

    public int GetAllGuestbookEntriesCounted()
    {
        try
        {
            return _entityContext.TblGbs.Count();
        }
        catch (Exception ex)
        {
            Log.Error($"Error when counting all GB-posts. Error-message: {ex.Message}");
            return 0;
        }
    }

    // Async methods
    public async Task<IEnumerable<TblGb>> GetAllGuestbookEntriesAsync()
    {
        return await Task.FromResult(GetAll());
    }

    public async Task<TblGb> GetOneGbEntryAsync(int id)
    {
        try
        {
            var entry = _entityContext.TblGbs.FirstOrDefault(gb => gb.GbId == id);
            return await Task.FromResult(entry ?? new TblGb { GbId = -1 });
        }
        catch (Exception ex)
        {
            Log.Error($"Error getting GB entry with id: {id}. Error-message: {ex.Message}");
            return await Task.FromResult(new TblGb { GbId = -1 });
        }
    }

    public async Task<bool> CreateGbEntryAsync(TblGb gb)
    {
        return await Task.FromResult(CreateGBpost(gb));
    }

    public async Task<bool> DeleteGbEntryAsync(int id)
    {
        return await Task.FromResult(DeleteGbPost(id));
    }
}
