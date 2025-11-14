using ArvidsonFoto.Data;
using ArvidsonFoto.Models;

namespace ArvidsonFoto.Services;

public class NewsService : INewsService
{
    // Databas koppling
    private readonly ArvidsonFotoDbContext _entityContext;

    public NewsService(ArvidsonFotoDbContext context)
    {
        _entityContext = context;
    }

    public List<TblNews> GetAll()
    {
        List<TblNews> newsList;
        newsList = _entityContext.TblNews.OrderByDescending(n => n.NewsCreated).ToList();
        return newsList;
    }

    public List<TblNews> GetPublished()
    {
        List<TblNews> newsList;
        newsList = _entityContext.TblNews
            .Where(n => n.NewsPublished)
            .OrderByDescending(n => n.NewsCreated)
            .ToList();
        return newsList;
    }

    public List<TblNews> GetLatestPublished(int count)
    {
        List<TblNews> newsList;
        newsList = _entityContext.TblNews
            .Where(n => n.NewsPublished)
            .OrderByDescending(n => n.NewsCreated)
            .Take(count)
            .ToList();
        return newsList;
    }

    public TblNews? GetById(int id)
    {
        return _entityContext.TblNews.FirstOrDefault(n => n.Id == id);
    }

    public TblNews? GetByNewsId(int newsId)
    {
        return _entityContext.TblNews.FirstOrDefault(n => n.NewsId == newsId);
    }

    public bool CreateNews(TblNews news)
    {
        bool succeeded = false;
        try
        {
            _entityContext.TblNews.Add(news);
            _entityContext.SaveChanges();
            succeeded = true;
        }
        catch (Exception ex)
        {
            Log.Error($"Fel vid skapande av nyhetsartikel: {ex.Message}");
        }
        return succeeded;
    }

    public bool UpdateNews(TblNews news)
    {
        bool succeeded = false;
        try
        {
            _entityContext.TblNews.Update(news);
            _entityContext.SaveChanges();
            succeeded = true;
        }
        catch (Exception ex)
        {
            Log.Error($"Fel vid uppdatering av nyhetsartikel: {ex.Message}");
        }
        return succeeded;
    }

    public bool DeleteNews(int id)
    {
        bool succeeded = false;
        try
        {
            var news = _entityContext.TblNews.FirstOrDefault(n => n.Id == id);
            if (news != null)
            {
                _entityContext.TblNews.Remove(news);
                _entityContext.SaveChanges();
                succeeded = true;
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Fel vid borttagning av nyhetsartikel: {ex.Message}");
        }
        return succeeded;
    }

    public int GetLastId()
    {
        var lastNews = _entityContext.TblNews.OrderByDescending(n => n.NewsId).FirstOrDefault();
        return lastNews?.NewsId ?? 0;
    }
}