namespace ArvidsonFoto.Controllers;

public class SenastController(ArvidsonFotoDbContext context) : Controller
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
            List<TblMenu> categories = _categoryService.GetAll().OrderBy(c => c.MenuText).ToList();
            viewModel.AllImagesList = new List<TblImage>();
            foreach (var category in categories)
            {
                TblImage image = _imageService.GetOneImageFromCategory(category.MenuId);
                image.Name = category.MenuText;
                viewModel.AllImagesList.Add(image);
            }
        }
        else if (sortOrder.Equals("Uppladdad"))
        {
            ViewData["Title"] = "Uppladdad";
            if (User?.Identity?.IsAuthenticated is false)
                _pageCounterService.AddPageCount("Senast-Uppladdad");
            viewModel.AllImagesList = _imageService.GetAll().OrderByDescending(i => i.ImageUpdate).ToList();
        }
        else if (sortOrder.Equals("Fotograferad"))
        {
            ViewData["Title"] = "Fotograferad";
            if (User?.Identity?.IsAuthenticated is false)
                _pageCounterService.AddPageCount("Senast-Fotograferad");
            viewModel.AllImagesList = _imageService.GetAll().OrderByDescending(i => i.ImageDate).ToList();
        }
        else
        {
            var url = Url.ActionContext.HttpContext;
            string visitedUrl = HttpRequestExtensions.GetRawUrl(url);
            Log.Information($"Redirect from page: {visitedUrl}, to page: /Senast/Fotograferad");

            return RedirectToAction("Index", new { sortOrder = "Fotograferad" });
        }

        viewModel.SelectedCategory = new TblMenu() { MenuText = sortOrder }; //Lägger till en SelectedCategory, så det inte blir tolkat som startsidan. 
        viewModel.DisplayImagesList = viewModel.AllImagesList.Skip(viewModel.CurrentPage * pageSize).Take(pageSize).ToList();
        viewModel.TotalPages = (int)Math.Ceiling(viewModel.AllImagesList.Count() / (decimal)pageSize);
        viewModel.CurrentPage = (int)sida;
        viewModel.CurrentUrl = "/Senast/" + sortOrder;

        return View(viewModel);
    }
}