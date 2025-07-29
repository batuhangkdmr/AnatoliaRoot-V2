using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AnatoliaRoot_V2.Data;
using AnatoliaRoot_V2.Services;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.SqlServer;
using System;

namespace AnatoliaRoot_V2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICloudinaryService, CloudinaryService>();

            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"),
                          new SqlServerStorageOptions
                          {
                              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                              QueuePollInterval = TimeSpan.Zero,
                              UseRecommendedIsolationLevel = true,
                              DisableGlobalLocks = true
                          })
            );
            services.AddHangfireServer();
            services.AddScoped<ExchangeRateService>();
            services.AddScoped<GoldPriceService>();

            services.AddAuthentication("AdminCookie")
                .AddCookie("AdminCookie", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/Login";
                    options.Cookie.Name = "AnatoliaRootAdminAuth";
                    options.ExpireTimeSpan = System.TimeSpan.FromHours(8);
                });

            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHangfireDashboard("/hangfire");
            RecurringJob.AddOrUpdate<ExchangeRateService>(
                "kur-cekme-job",
                service => service.FetchAndSaveRatesAsync(),
                Cron.Hourly
            );
            RecurringJob.AddOrUpdate<ExchangeRateService>(
                "kur-temizle-job",
                service => service.PruneOldRatesAsync(),
                Cron.Daily
            );
            RecurringJob.AddOrUpdate<GoldPriceService>(
                "altin-cekme-job",
                service => service.FetchAndSaveGoldPricesAsync(),
                "0 9 * * *" // Her gün saat 09:00'da
            );
            RecurringJob.AddOrUpdate<GoldPriceService>(
                "altin-temizle-job",
                service => service.PruneOldGoldPricesAsync(),
                Cron.Daily
            );
        }
    }
} 