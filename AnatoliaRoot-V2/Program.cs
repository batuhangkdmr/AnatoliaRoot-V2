using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using AnatoliaRoot_V2.Data;
using Microsoft.EntityFrameworkCore;

namespace AnatoliaRoot_V2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    
                    // Veritabanının var olup olmadığını kontrol et
                    if (!context.Database.CanConnect())
                    {
                        context.Database.EnsureCreated();
                    }
                    else
                    {
                        // Migration history tablosuna manuel kayıt ekle
                        var connection = context.Database.GetDbConnection();
                        connection.Open();
                        
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = @"
                                IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250715162127_InitialCreate')
                                BEGIN
                                    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) 
                                    VALUES ('20250715162127_InitialCreate', '3.1.32')
                                END";
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while setting up the database.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}