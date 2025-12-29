namespace ArvidsonFoto.Controllers;

/// <summary>
/// Controller for development/admin landing page
/// </summary>
public class DevController : Controller
{
    /// <summary>
    /// Landing page that shows links to API documentation and Admin panel
    /// </summary>
    [Route("/dev")]
    [Route("/admin")]
    public IActionResult Index()
    {
        // Only accessible in Development mode
        if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            return NotFound();
        }

        return View();
    }
}
