using ArvidsonFoto.Core.Data;
using ArvidsonFoto.Core.ViewModels;
using ArvidsonFoto.Services;
using ArvidsonFoto.Views.Shared;
using ArvidsonFoto.Mappers;

namespace ArvidsonFoto.Controllers;

public class SenastController(ArvidsonFotoCoreDbContext context) : Controller
{
    internal IImageService _imageService = new ImageService(context);
    internal ICategoryService _categoryService = new CategoryService(context);
    internal IPageCounterService _pageCounterService = new PageCounterService(context);

    [Route("[controller]/{sortOrder}")]
    public IActionResult Index(string sortOrder, int? sida)
    {
        GalleryViewModel viewModel = new GalleryViewModel();
        int pageSize = 48;

        if (sida is null || sida < 1)
            sida = 1;

        if (sortOrder is null)
            sortOrder = "Fotograferad";

        viewModel.CurrentPage = (int)sida - 1;

        if (sortOrder.Equals("Per kategori"))
        {
            ViewData["Title"] = "Per kategori";
            if (User?.Identity?.IsAuthenticated is false)
                _pageCounterService.AddPageCount("Senast-Per kategori");
            
            List<Core.Models.TblMenu> categories = _categoryService.GetAll().OrderBy(c => c.MenuDisplayName).ToList();
            viewModel.AllImagesList = new List<Core.DTOs.ImageDto>();
            
            foreach (var category in categories)
            {
                if (category.MenuCategoryId.HasValue)
                {
                    Core.Models.TblImage image = _imageService.GetOneImageFromCategory(category.MenuCategoryId.Value);
                    var imageDto = DtoMapper.MapToImageDto(image, _categoryService);
                    imageDto.Name = category.MenuDisplayName ?? string.Empty;
                    viewModel.AllImagesList.Add(imageDto);
                }
            }
        }
        else if (sortOrder.Equals("Uppladdad"))
        {
            ViewData["Title"] = "Uppladdad";
            if (User?.Identity?.IsAuthenticated is false)
                _pageCounterService.AddPageCount("Senast-Uppladdad");
            
            var images = _imageService.GetAll().OrderByDescending(i => i.ImageUpdate).ToList();
            viewModel.AllImagesList = images.Select(i => DtoMapper.MapToImageDto(i, _categoryService)).ToList();
        }
        else if (sortOrder.Equals("Fotograferad"))
        {
            ViewData["Title"] = "Fotograferad";
            if (User?.Identity?.IsAuthenticated is false)
                _pageCounterService.AddPageCount("Senast-Fotograferad");
            
            var images = _imageService.GetAll().OrderByDescending(i => i.ImageDate).ToList();
            viewModel.AllImagesList = images.Select(i => DtoMapper.MapToImageDto(i, _categoryService)).ToList();
        }
        else
        {
            var url = Url.ActionContext.HttpContext;
            string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
            Log.Information($"Redirect from page: {visitedUrl}, to page: /Senast/Fotograferad");

            return RedirectToAction("Index", new { sortOrder = "Fotograferad" });
        }

        viewModel.SelectedCategory = new Core.DTOs.CategoryDto { Name = sortOrder };
        viewModel.DisplayImagesList = viewModel.AllImagesList.Skip(viewModel.CurrentPage * pageSize).Take(pageSize).ToList();
        viewModel.TotalPages = (int)Math.Ceiling(viewModel.AllImagesList.Count() / (decimal)pageSize);
        viewModel.CurrentPage = (int)sida;
        viewModel.CurrentUrl = "/Senast/" + sortOrder;

        return View(viewModel);
    }
}