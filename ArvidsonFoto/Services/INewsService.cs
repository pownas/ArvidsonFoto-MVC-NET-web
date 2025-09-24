using ArvidsonFoto.Models;

namespace ArvidsonFoto.Services;

public interface INewsService
{
    List<TblNews> GetAll();
    List<TblNews> GetPublished();
    List<TblNews> GetLatestPublished(int count);
    TblNews? GetById(int id);
    TblNews? GetByNewsId(int newsId);
    bool CreateNews(TblNews news);
    bool UpdateNews(TblNews news);
    bool DeleteNews(int id);
    int GetLastId();
}