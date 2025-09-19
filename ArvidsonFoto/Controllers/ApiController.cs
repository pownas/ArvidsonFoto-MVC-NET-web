namespace ArvidsonFoto.Controllers;

[ApiController]
[Route("api")]
public class ApiController(ArvidsonFotoDbContext context) : ControllerBase
{
    private readonly string _imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Bilder");
    private readonly IImageService _imageService = new ImageService(context);
    private readonly ICategoryService _categoryService = new CategoryService(context);

    // GET: api/images
    [HttpGet("images")]
    public ActionResult<IEnumerable<string>> GetImages()
    {
        if (!Directory.Exists(_imagesPath))
            return NotFound("Images directory not found.");

        var images = Directory.GetFiles(_imagesPath)
            .Select(Path.GetFileName)
            .Where(f => f != null && (
                f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return Ok(images);
    }

    // GET: api/image/{filename}
    [HttpGet("image/{filename}")]
    public IActionResult GetImage(string filename)
    {
        var filePath = Path.Combine(_imagesPath, filename);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var ext = Path.GetExtension(filename).ToLowerInvariant();
        var contentType = ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };

        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, contentType);
    }
}