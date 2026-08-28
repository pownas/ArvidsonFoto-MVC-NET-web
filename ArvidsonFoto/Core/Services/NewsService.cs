using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.Models;
using System.Net;
using System.Text.RegularExpressions;

namespace ArvidsonFoto.Core.Services;

public class NewsService : INewsService
{
    private const string NewsImageBaseUrl = "https://arvidsonfoto.se/bilder";

    // Databas koppling
    private readonly ArvidsonFotoCoreDbContext _entityContext;
    private readonly IApiCategoryService _categoryService;

    public NewsService(ArvidsonFotoCoreDbContext context, IApiCategoryService categoryService)
    {
        _entityContext = context;
        _categoryService = categoryService;
    }

    public List<TblNews> GetAll()
    {
        return PopulatePresentationFields(_entityContext.TblNews
            .OrderByDescending(n => n.NewsCreated)
            .ToList());
    }

    public List<TblNews> GetPublished()
    {
        return PopulatePresentationFields(_entityContext.TblNews
            .Where(n => n.NewsPublished)
            .OrderByDescending(n => n.NewsCreated)
            .ToList());
    }

    public List<TblNews> GetLatestPublished(int count)
    {
        return PopulatePresentationFields(_entityContext.TblNews
            .Where(n => n.NewsPublished)
            .OrderByDescending(n => n.NewsCreated)
            .Take(count)
            .ToList());
    }

    public TblNews? GetById(int id)
    {
        return PopulatePresentationFields(_entityContext.TblNews.FirstOrDefault(n => n.Id == id));
    }

    public TblNews? GetByNewsId(int newsId)
    {
        return PopulatePresentationFields(_entityContext.TblNews.FirstOrDefault(n => n.NewsId == newsId));
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

    private List<TblNews> PopulatePresentationFields(List<TblNews> newsList)
    {
        if (newsList.Count == 0)
        {
            return newsList;
        }

        var imageIds = newsList
            .Where(n => n.NewsImageId.HasValue && n.NewsImageId.Value > 0)
            .Select(n => n.NewsImageId!.Value)
            .Distinct()
            .ToList();

        var imagesById = imageIds.Count == 0
            ? new Dictionary<int, TblImage>()
            : _entityContext.TblImages
                .Where(i => i.ImageId.HasValue && imageIds.Contains(i.ImageId.Value))
                .ToDictionary(i => i.ImageId!.Value, i => i);

        foreach (var news in newsList)
        {
            PopulatePresentationFields(news, imagesById);
        }

        return newsList;
    }

    private TblNews? PopulatePresentationFields(TblNews? news)
    {
        if (news == null)
        {
            return null;
        }

        var imagesById = news.NewsImageId.HasValue && news.NewsImageId.Value > 0
            ? _entityContext.TblImages
                .Where(i => i.ImageId == news.NewsImageId)
                .ToDictionary(i => i.ImageId ?? 0, i => i)
            : new Dictionary<int, TblImage>();

        PopulatePresentationFields(news, imagesById);
        return news;
    }

    private void PopulatePresentationFields(TblNews news, IReadOnlyDictionary<int, TblImage> imagesById)
    {
        news.NewsExcerpt = CreateExcerpt(news);
        news.NewsImageUrl = null;
        news.NewsImageThumbnailUrl = null;
        news.NewsImageAlt = news.NewsTitle;
        news.NewsImageDescription = null;

        if (!news.NewsImageId.HasValue || news.NewsImageId.Value <= 0 || !imagesById.TryGetValue(news.NewsImageId.Value, out var image))
        {
            return;
        }

        var categoryPath = image.ImageCategoryId.HasValue
            ? _categoryService.GetCategoryPathForImage(image.ImageCategoryId.Value)
            : string.Empty;

        if (string.IsNullOrWhiteSpace(categoryPath) || string.IsNullOrWhiteSpace(image.ImageUrlName))
        {
            return;
        }

        var imageUrl = $"{NewsImageBaseUrl}/{categoryPath}/{image.ImageUrlName}";
        var imageDescription = string.IsNullOrWhiteSpace(image.ImageDescription)
            ? news.NewsTitle
            : image.ImageDescription;

        news.NewsImageUrl = imageUrl;
        news.NewsImageThumbnailUrl = $"{imageUrl}.thumb.jpg";
        news.NewsImageAlt = imageDescription;
        news.NewsImageDescription = imageDescription;
    }

    private static string CreateExcerpt(TblNews news)
    {
        if (!string.IsNullOrWhiteSpace(news.NewsSummary))
        {
            return news.NewsSummary.Trim();
        }

        if (string.IsNullOrWhiteSpace(news.NewsContent))
        {
            return string.Empty;
        }

        var plainText = Regex.Replace(news.NewsContent, "<.*?>", " ");
        plainText = WebUtility.HtmlDecode(plainText);
        plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

        if (plainText.Length <= 180)
        {
            return plainText;
        }

        return $"{plainText[..177].TrimEnd()}...";
    }
}