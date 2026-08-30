using AnatoliaRoot_V2.Models;
using AnatoliaRoot_V2.Models.ViewModels;
using AnatoliaRoot_V2.Data;
using AnatoliaRoot_V2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace AnatoliaRoot_V2.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILoginRateLimiter _loginRateLimiter;

        public AccountController(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher,
            ILoginRateLimiter loginRateLimiter)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _loginRateLimiter = loginRateLimiter;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var utcNow = DateTime.UtcNow;
            var clientKey = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (!_loginRateLimiter.TryAcquire(clientKey, utcNow, out var retryAfter))
            {
                Response.StatusCode = 429;
                Response.Headers["Retry-After"] = Math.Ceiling(retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
                ModelState.AddModelError("", "Çok fazla giriş denemesi yapıldı. Lütfen daha sonra tekrar deneyin.");
                return View(model);
            }

            var normalizedUsername = model.Username.Trim().ToUpperInvariant();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUsername == normalizedUsername);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            if (user.LockoutEndUtc.HasValue && user.LockoutEndUtc.Value > utcNow)
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            if (user.LockoutEndUtc.HasValue)
            {
                user.FailedLoginAttempts = 0;
                user.LockoutEndUtc = null;
                await _context.SaveChangesAsync();
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                await RecordFailedLoginAsync(user.Id);
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            user.FailedLoginAttempts = 0;
            user.LockoutEndUtc = null;
            if (string.IsNullOrWhiteSpace(user.SecurityStamp))
                user.SecurityStamp = Guid.NewGuid().ToString();

            await _context.SaveChangesAsync();

            // Cookie authentication ile oturum aç
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(AnatoliaRoot_V2.Models.User.SecurityStampClaimType, user.SecurityStamp)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "AdminCookie");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };
            await HttpContext.SignInAsync("AdminCookie", new ClaimsPrincipal(claimsIdentity), authProperties);
            return RedirectToAction("Index", "Category");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            return RedirectToAction("Login");
        }

        private Task<int> RecordFailedLoginAsync(int userId)
        {
            return _context.Database.ExecuteSqlRawAsync(@"
                UPDATE [Users]
                SET [FailedLoginAttempts] = [FailedLoginAttempts] + 1,
                    [LockoutEndUtc] = CASE
                        WHEN [FailedLoginAttempts] + 1 >= 5 THEN DATEADD(MINUTE, 15, SYSUTCDATETIME())
                        ELSE [LockoutEndUtc]
                    END
                WHERE [Id] = {0}", userId);
        }

    }
}
