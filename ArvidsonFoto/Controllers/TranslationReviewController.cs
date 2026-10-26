using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.Extensions;
using ArvidsonFoto.Core.Interfaces;
using ArvidsonFoto.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ArvidsonFoto.Controllers;

[Authorize]
public class TranslationReviewController(ArvidsonFotoCoreDbContext db, IApiCategoryService? categoryService = null) : Controller
{
    private const int PageSize = 20;

    public IActionResult Index(string table = "menu", bool pendingOnly = true, int page = 1, bool missingOnly = false, string? search = null)
    {
        if (table is not ("menu" or "images" or "guestbook"))
        {
            return BadRequest();
        }
        search = table == "images" ? search?.Trim() : null;
        if (search?.Length > 100)
        {
            return BadRequest();
        }

        var model = new TranslationReviewViewModel
        {
            Table = table,
            PendingOnly = pendingOnly,
            MissingOnly = table == "images" && missingOnly,
            Search = search,
            Page = Math.Max(1, page),
            AllPendingCount = db.TblMenus.Count(m => !m.MenuDisplayNameEnReviewed) +
                db.TblMenus.Count(m => !m.MenuUrlSegmentEnReviewed) +
                db.TblImages.Count(i => i.ImageDescription != null && i.ImageDescription.Trim() != "" && !i.ImageDescriptionEnReviewed) +
                db.TblGbs.Count(g => !g.GbNameEnReviewed) +
                db.TblGbs.Count(g => !g.GbTextEnReviewed),
            AllFieldsCount = (2 * db.TblMenus.Count()) +
                db.TblImages.Count(i => i.ImageDescription != null && i.ImageDescription.Trim() != "") +
                (2 * db.TblGbs.Count())
        };
        if (table == "menu")
        {
            var query = db.TblMenus.AsNoTracking().Where(m => m.MenuCategoryId != null);
            model.PendingCount = query.Count(m => !m.MenuDisplayNameEnReviewed) +
                query.Count(m => !m.MenuUrlSegmentEnReviewed);
            model.TotalCount = query.Count() * 2;
            if (pendingOnly)
            {
                query = query.Where(m => !m.MenuDisplayNameEnReviewed || !m.MenuUrlSegmentEnReviewed);
            }
            model.TotalPages = (int)Math.Ceiling(query.Count() / (double)PageSize);
            var rows = query.OrderBy(m => m.MenuCategoryId).Skip((model.Page - 1) * PageSize).Take(PageSize).ToList();
            foreach (var row in rows)
            {
                var recordName = LocalizedText.Select(row.MenuDisplayName, row.MenuDisplayNameEn);
                model.Items.Add(new(row.MenuCategoryId!.Value, "name", row.MenuDisplayName ?? "", row.MenuDisplayNameEn, row.MenuDisplayNameEnReviewed, recordName));
                model.Items.Add(new(row.MenuCategoryId.Value, "url", row.MenuUrlSegment ?? "", row.MenuUrlSegmentEn, row.MenuUrlSegmentEnReviewed, recordName));
            }
        }
        else if (table == "images")
        {
            var query = db.TblImages.AsNoTracking()
                .Where(i => i.ImageId != null && i.ImageDescription != null && i.ImageDescription.Trim() != "");
            if (!string.IsNullOrEmpty(search))
            {
                var imageId = int.TryParse(search, out var parsedId) ? parsedId : -1;
                query = query.Where(i => i.ImageId == imageId ||
                    (i.ImageUrlName != null && i.ImageUrlName.Contains(search)) ||
                    (i.ImageDescription != null && i.ImageDescription.Contains(search)));
            }
            model.PendingCount = query.Count(i => !i.ImageDescriptionEnReviewed);
            model.MissingCount = query.Count(i => i.ImageDescriptionEn == null || i.ImageDescriptionEn.Trim() == "");
            model.TotalCount = query.Count();
            if (missingOnly)
            {
                query = query.Where(i => i.ImageDescriptionEn == null || i.ImageDescriptionEn.Trim() == "");
            }
            else if (pendingOnly)
            {
                query = query.Where(i => !i.ImageDescriptionEnReviewed);
            }
            model.TotalPages = (int)Math.Ceiling(query.Count() / (double)PageSize);
            model.Page = Math.Min(model.Page, Math.Max(1, model.TotalPages));
            model.Items = [.. query.OrderBy(i => i.ImageId).Skip((model.Page - 1) * PageSize).Take(PageSize).Select(i => new TranslationReviewItem(i.ImageId!.Value, "description", i.ImageDescription ?? "", i.ImageDescriptionEn, i.ImageDescriptionEnReviewed))];
        }
        else
        {
            var query = db.TblGbs.AsNoTracking();
            model.PendingCount = query.Count(g => !g.GbNameEnReviewed) + query.Count(g => !g.GbTextEnReviewed);
            model.TotalCount = query.Count() * 2;
            if (pendingOnly)
            {
                query = query.Where(g => !g.GbNameEnReviewed || !g.GbTextEnReviewed);
            }
            model.TotalPages = (int)Math.Ceiling(query.Count() / (double)PageSize);
            var rows = query.OrderBy(g => g.Id).Skip((model.Page - 1) * PageSize).Take(PageSize).ToList();
            foreach (var row in rows)
            {
                model.Items.Add(new(row.Id, "name", row.GbName ?? "", row.GbNameEn, row.GbNameEnReviewed));
                model.Items.Add(new(row.Id, "text", row.GbText ?? "", row.GbTextEn, row.GbTextEnReviewed));
            }
        }
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Save(string table, int id, string field, string? english, string swedish, string? previousEnglish, bool reviewed, bool pendingOnly = true, int page = 1, bool missingOnly = false, string? search = null)
    {
        if (table is not ("menu" or "images" or "guestbook") || id < 1)
        {
            return BadRequest();
        }

        string? originalSv;
        string? originalEn;
        Action<string?, bool> update;
        switch (table, field)
        {
            case ("menu", "name"):
                var menuName = db.TblMenus.FirstOrDefault(m => m.MenuCategoryId == id);
                if (menuName is null)
                {
                    return NotFound();
                }
                originalSv = menuName.MenuDisplayName;
                originalEn = menuName.MenuDisplayNameEn;
                update = (value, status) => { menuName.MenuDisplayNameEn = value; menuName.MenuDisplayNameEnReviewed = status; };
                break;
            case ("menu", "url"):
                var menuUrl = db.TblMenus.FirstOrDefault(m => m.MenuCategoryId == id);
                if (menuUrl is null)
                {
                    return NotFound();
                }
                originalSv = menuUrl.MenuUrlSegment;
                originalEn = menuUrl.MenuUrlSegmentEn;
                update = (value, status) => { menuUrl.MenuUrlSegmentEn = value; menuUrl.MenuUrlSegmentEnReviewed = status; };
                break;
            case ("images", "description"):
                var image = db.TblImages.FirstOrDefault(i => i.ImageId == id);
                if (image is null)
                {
                    return NotFound();
                }
                originalSv = image.ImageDescription;
                originalEn = image.ImageDescriptionEn;
                update = (value, status) => { image.ImageDescriptionEn = value; image.ImageDescriptionEnReviewed = status; };
                break;
            case ("guestbook", "name"):
                var guestName = db.TblGbs.FirstOrDefault(g => g.Id == id);
                if (guestName is null)
                {
                    return NotFound();
                }
                originalSv = guestName.GbName;
                originalEn = guestName.GbNameEn;
                update = (value, status) => { guestName.GbNameEn = value; guestName.GbNameEnReviewed = status; };
                break;
            case ("guestbook", "text"):
                var guestText = db.TblGbs.FirstOrDefault(g => g.Id == id);
                if (guestText is null)
                {
                    return NotFound();
                }
                originalSv = guestText.GbText;
                originalEn = guestText.GbTextEn;
                update = (value, status) => { guestText.GbTextEn = value; guestText.GbTextEnReviewed = status; };
                break;
            default:
                return BadRequest();
        }

        if ((originalSv ?? "") != (swedish ?? "") || (originalEn ?? "") != (previousEnglish ?? ""))
        {
            return Conflict();
        }

        var value = string.IsNullOrWhiteSpace(english) ? null : english.Trim();
        if (reviewed && value is null)
        {
            return BadRequest();
        }
        var maxLength = (table, field) switch
        {
            ("menu", _) => 50,
            ("images", _) => 150,
            ("guestbook", "name") => 100,
            _ => int.MaxValue
        };
        if (value?.Length > maxLength || (field == "url" && value is not null &&
            (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9-]+$") ||
             db.TblMenus.Any(m => m.MenuCategoryId != id && (m.MenuUrlSegment == value || m.MenuUrlSegmentEn == value)))))
        {
            return BadRequest();
        }

        var reviewStatus = reviewed && value is not null && value == originalEn;
        if (db.Database.IsRelational())
        {
            // Villkoren kontrolleras i samma UPDATE som granskningsmarkeringen.
            var updated = (table, field) switch
            {
                ("menu", "name") => db.TblMenus.Where(m => m.MenuCategoryId == id && m.MenuDisplayName == originalSv && m.MenuDisplayNameEn == originalEn)
                    .ExecuteUpdate(s => s.SetProperty(m => m.MenuDisplayNameEn, value).SetProperty(m => m.MenuDisplayNameEnReviewed, reviewStatus)),
                ("menu", "url") => db.TblMenus.Where(m => m.MenuCategoryId == id && m.MenuUrlSegment == originalSv && m.MenuUrlSegmentEn == originalEn)
                    .ExecuteUpdate(s => s.SetProperty(m => m.MenuUrlSegmentEn, value).SetProperty(m => m.MenuUrlSegmentEnReviewed, reviewStatus)),
                ("images", "description") => db.TblImages.Where(i => i.ImageId == id && i.ImageDescription == originalSv && i.ImageDescriptionEn == originalEn)
                    .ExecuteUpdate(s => s.SetProperty(i => i.ImageDescriptionEn, value).SetProperty(i => i.ImageDescriptionEnReviewed, reviewStatus)),
                ("guestbook", "name") => db.TblGbs.Where(g => g.Id == id && g.GbName == originalSv && g.GbNameEn == originalEn)
                    .ExecuteUpdate(s => s.SetProperty(g => g.GbNameEn, value).SetProperty(g => g.GbNameEnReviewed, reviewStatus)),
                _ => db.TblGbs.Where(g => g.Id == id && g.GbText == originalSv && g.GbTextEn == originalEn)
                    .ExecuteUpdate(s => s.SetProperty(g => g.GbTextEn, value).SetProperty(g => g.GbTextEnReviewed, reviewStatus))
            };
            if (updated != 1)
            {
                return Conflict();
            }
        }
        else
        {
            update(value, reviewStatus);
            _ = db.SaveChanges();
        }
        if (table == "menu" && value != originalEn)
        {
            categoryService?.ClearCache();
        }
        return RedirectToAction(nameof(Index), new { table, pendingOnly, page, missingOnly, search });
    }
}
