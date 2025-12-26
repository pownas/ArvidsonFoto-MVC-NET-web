using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Interfaces;

namespace ArvidsonFoto.Core.Services;

/// <summary>
/// Guestbook service - manages guestbook entries
/// </summary>
public class GuestBookService : IGuestBookService
{
    private readonly ArvidsonFotoCoreDbContext _entityContext;

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
        bool succeeded = false;
        try
        {
            _entityContext.TblGbs.Add(gb);
            _entityContext.SaveChanges();
            succeeded = true;
        }
        catch (Exception ex)
        {
            succeeded = false;
            Log.Error("Error when creating GB-post. Error-message: " + ex.Message);
        }
        return succeeded;
    }

    public bool ReadGbPost(int gbId)
    {
        bool succeeded = false;
        try
        {
            TblGb? gb = _entityContext.TblGbs.FirstOrDefault(gb => gb.GbId == gbId);
            if (gb != null)
            {
                gb.GbReadPost = true;
                _entityContext.SaveChanges();
                succeeded = true;
            }
        }
        catch (Exception ex)
        {
            succeeded = false;
            Log.Error("Error when trying to mark GB-post with id: " + gbId + ", as read. Error-message: " + ex.Message);
        }
        return succeeded;
    }

    public bool DeleteGbPost(int gbId)
    {
        bool succeeded = false;
        try
        {
            TblGb? gb = _entityContext.TblGbs.FirstOrDefault(gb => gb.GbId == gbId);
            if (gb != null)
            {
                _entityContext.TblGbs.Remove(gb);
                _entityContext.SaveChanges();
                succeeded = true;
            }
        }
        catch (Exception ex)
        {
            succeeded = false;
            Log.Error("Error when deleting GB-post with id: " + gbId + ". Error-message: " + ex.Message);
        }
        return succeeded;
    }

    public int GetCountOfUnreadPosts()
    {
        int postCount = 0;
        try
        {
            postCount = _entityContext.TblGbs.Count(gb => gb.GbReadPost == null || gb.GbReadPost == false);
        }
        catch (Exception ex)
        {
            postCount = 0;
            Log.Error("Error when counting Unread GB-posts. Error-message: " + ex.Message);
        }
        return postCount;
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
}
