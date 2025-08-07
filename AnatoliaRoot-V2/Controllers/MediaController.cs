using Microsoft.AspNetCore.Mvc;

namespace AnatoliaRoot_V2.Controllers
{
    [Route("medya")]
    public class MediaController : Controller
    {
        [Route("galeri")]
        public IActionResult Gallery()
        {
            ViewData["Title"] = "Resim Galerisi";
            return View();
        }
    }
} 