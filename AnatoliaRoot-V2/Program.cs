using System;
using System.Linq;
using AnatoliaRoot_V2.Data;
using AnatoliaRoot_V2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AnatoliaRoot_V2
{
    public class Program
    {
        private const string BootstrapAdminArgument = "--bootstrap-admin";

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            if (args.Any(argument => string.Equals(argument, BootstrapAdminArgument, StringComparison.OrdinalIgnoreCase)))
            {
                BootstrapAdmin(host.Services);
                return;
            }

            EnsureDatabaseReady(host.Services);
            host.Run();
        }

        private static void EnsureDatabaseReady(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!context.Database.CanConnect())
                throw new InvalidOperationException("Veritabanına bağlanılamadı. Bağlantı bilgisini ve ağ erişimini kontrol edin.");

            var pendingMigrations = context.Database.GetPendingMigrations().ToArray();
            if (pendingMigrations.Length > 0)
            {
                throw new InvalidOperationException(
                    $"Veritabanında uygulanmamış migration bulunuyor: {string.Join(", ", pendingMigrations)}. Uygulamayı başlatmadan önce migration'ları uygulayın.");
            }
        }

        private static void BootstrapAdmin(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<User>>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            if (!context.Database.CanConnect())
                throw new InvalidOperationException("Admin oluşturulamadı: veritabanına bağlanılamıyor.");

            var pendingMigrations = context.Database.GetPendingMigrations().ToArray();
            if (pendingMigrations.Length > 0)
                throw new InvalidOperationException("Admin oluşturmadan önce tüm migration'ları uygulayın.");

            if (context.Users.Any())
                throw new InvalidOperationException("Admin bootstrap yalnızca Users tablosu boşken çalıştırılabilir.");

            var username = configuration["AdminBootstrap:Username"]?.Trim();
            var password = configuration["AdminBootstrap:Password"];

            if (string.IsNullOrWhiteSpace(username) || username.Length > 50)
                throw new InvalidOperationException("AdminBootstrap__Username zorunludur ve en fazla 50 karakter olabilir.");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
                throw new InvalidOperationException("AdminBootstrap__Password en az 12 karakter olmalıdır.");

            var user = new User
            {
                Username = username,
                NormalizedUsername = username.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString()
            };
            user.PasswordHash = passwordHasher.HashPassword(user, password);

            context.Users.Add(user);
            context.SaveChanges();
            logger.LogInformation("İlk admin hesabı oluşturuldu. Bootstrap environment değişkenlerini kaldırın.");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
