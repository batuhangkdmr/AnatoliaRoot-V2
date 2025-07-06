using Microsoft.AspNetCore.Mvc;

namespace AnatoliaRoot_V2.Controllers
{
    public class MediaController : Controller
    {
        public IActionResult Gallery()
        {
            ViewData["Title"] = "Resim Galerisi";
            return View();
        }
    }
} 