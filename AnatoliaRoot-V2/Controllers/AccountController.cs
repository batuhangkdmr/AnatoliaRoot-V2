using AnatoliaRoot_V2.Models;
using AnatoliaRoot_V2.Models.ViewModels;
using AnatoliaRoot_V2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace AnatoliaRoot_V2.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var secretKey = _configuration["AdminSecretKey"];
            if (model.SecretKey != secretKey)
            {
                ModelState.AddModelError("SecretKey", "Geçersiz admin anahtarı.");
                return View(model);
            }

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Bu kullanıcı adı zaten alınmış.");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                PasswordHash = HashPassword(model.Password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // Kayıt sonrası login sayfasına yönlendir
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);
            if (user == null || user.PasswordHash != HashPassword(model.Password))
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }
            // Cookie authentication ile oturum aç
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var claimsIdentity = new ClaimsIdentity(claims, "AdminCookie");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };
            await HttpContext.SignInAsync("AdminCookie", new ClaimsPrincipal(claimsIdentity), authProperties);
            return RedirectToAction("Index", "Category");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = "AdminCookie")]
        public IActionResult AdminPanel()
        {
            return Content("Sadece giriş yapan admin görebilir.");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
} 