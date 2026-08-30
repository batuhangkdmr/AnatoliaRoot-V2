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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using AnatoliaRoot_V2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("ConnectionStrings__DefaultConnection yapılandırması zorunludur.");

            services.AddControllersWithViews(options =>
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            services.AddMemoryCache(options => options.SizeLimit = 10000);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString, sqlServerOptions =>
                    sqlServerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddSingleton<ILoginRateLimiter, LoginRateLimiter>();
            services.AddHttpClient("ExchangeRate", client => client.Timeout = TimeSpan.FromSeconds(15));
            services.AddHttpClient("GoldPrice", client => client.Timeout = TimeSpan.FromSeconds(15));

            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UseSqlServerStorage(connectionString,
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
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                    options.SlidingExpiration = false;
                    options.Events.OnValidatePrincipal = async context =>
                    {
                        var userIdValue = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        var securityStamp = context.Principal?.FindFirstValue(User.SecurityStampClaimType);

                        if (!int.TryParse(userIdValue, out var userId) || string.IsNullOrWhiteSpace(securityStamp))
                        {
                            context.RejectPrincipal();
                            await context.HttpContext.SignOutAsync("AdminCookie");
                            return;
                        }

                        var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(item => item.Id == userId);
                        var isValid = user != null
                            && string.Equals(user.SecurityStamp, securityStamp, StringComparison.Ordinal);

                        if (!isValid)
                        {
                            context.RejectPrincipal();
                            await context.HttpContext.SignOutAsync("AdminCookie");
                        }
                    };
                });

            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

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

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

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
