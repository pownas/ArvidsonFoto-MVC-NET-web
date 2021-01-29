using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArvidsonFoto.Controllers
{
    [Authorize]
    public class UploadAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
