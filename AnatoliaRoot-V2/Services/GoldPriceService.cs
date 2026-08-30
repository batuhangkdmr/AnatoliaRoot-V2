using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using AnatoliaRoot_V2.Models;
using Microsoft.EntityFrameworkCore;
using AnatoliaRoot_V2.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AnatoliaRoot_V2.Services
{
    public class GoldPriceService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GoldPriceService> _logger;
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly string _apiHost;

        public GoldPriceService(
            AppDbContext context,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<GoldPriceService> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _apiKey = configuration["ExternalApis:GoldPrice:ApiKey"];
            _apiUrl = configuration["ExternalApis:GoldPrice:ApiUrl"];
            _apiHost = configuration["ExternalApis:GoldPrice:ApiHost"];

        }

        public async Task FetchAndSaveGoldPricesAsync()
        {
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_apiUrl) || string.IsNullOrWhiteSpace(_apiHost))
                throw new InvalidOperationException("Altın API yapılandırması eksik.");

            _logger.LogInformation("Altın fiyatları çekiliyor.");
            var client = _httpClientFactory.CreateClient("GoldPrice");

                    // Harem Altın API için gerekli header'ları ekle
                    client.DefaultRequestHeaders.Add("X-RapidAPI-Key", _apiKey);
                    client.DefaultRequestHeaders.Add("X-RapidAPI-Host", _apiHost);

                    // Query parametreleri ekle
                    var url = $"{_apiUrl}?type=gold&code=gramaltin,CEYREKALTIN,YARIMALTIN,TAMALTIN,ONSALTIN";

                    var response = await client.GetStringAsync(url);
                    using var jsonDoc = JsonDocument.Parse(response);

                    // Harem Altın API response'unu parse et
                    if (jsonDoc.RootElement.GetProperty("status").GetString() == "success")
                    {
                        var data = jsonDoc.RootElement.GetProperty("data");
                        
                        decimal? gramGold = null;
                        decimal? quarterGold = null;
                        decimal? halfGold = null;
                        decimal changeRate = 0;
                        decimal dayHigh = 0;
                        decimal dayLow = 0;
                        decimal prevClose = 0;

                        // Her altın türü için fiyatları al
                        foreach (var item in data.EnumerateArray())
                        {
                            var currencyCode = item.GetProperty("currencyCode").GetString();
                            var buy = item.GetProperty("buy").GetDecimal();
                            var change = item.GetProperty("changeRate").GetDecimal();
                            var high = item.GetProperty("dayHigh").GetDecimal();
                            var low = item.GetProperty("dayLow").GetDecimal();
                            var prev = item.GetProperty("prevClose").GetDecimal();

                            switch (currencyCode)
                            {
                                case "gramaltin":
                                    gramGold = buy;
                                    changeRate = change;
                                    dayHigh = high;
                                    dayLow = low;
                                    prevClose = prev;
                                    break;
                                case "CEYREKALTIN":
                                    quarterGold = buy;
                                    break;
                                case "YARIMALTIN":
                                    halfGold = buy;
                                    break;
                            }
                        }

                        var timestamp = jsonDoc.RootElement.GetProperty("systemTime").GetInt64();
                        if (timestamp <= 0)
                            throw new InvalidOperationException("Altın API geçersiz timestamp döndürdü.");

                        if (!gramGold.HasValue || gramGold.Value <= 0
                            || !quarterGold.HasValue || quarterGold.Value <= 0
                            || !halfGold.HasValue || halfGold.Value <= 0
                            || dayHigh <= 0 || dayLow <= 0 || dayLow > dayHigh || prevClose <= 0)
                        {
                            throw new InvalidOperationException("Altın API zorunlu fiyat alanlarını eksik veya geçersiz döndürdü.");
                        }

                        if (await _context.GoldPrices.AnyAsync(item => item.Timestamp == timestamp))
                            return;

                        var goldPrice = new GoldPrice
                        {
                            Date = DateTime.UtcNow,
                            Timestamp = timestamp,
                            
                            // Altın fiyatları
                            GramGold = gramGold.Value,
                            QuarterGold = quarterGold.Value,
                            HalfGold = halfGold.Value,
                            
                            // Değişim bilgileri
                            ChangeRate = changeRate,
                            DayHigh = dayHigh,
                            DayLow = dayLow,
                            PrevClose = prevClose,
                            
                            // API bilgileri
                            Source = "Harem Altın API",
                            Currency = "TRY",
                            Exchange = "Harem Altın"
                        };

                        _context.GoldPrices.Add(goldPrice);
                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateException)
                        {
                            _context.Entry(goldPrice).State = EntityState.Detached;
                            if (!await _context.GoldPrices.AnyAsync(item => item.Timestamp == timestamp))
                                throw;
                        }

                        _logger.LogInformation("Altın fiyatları kaydedildi. Kaynak timestamp: {Timestamp}", timestamp);
                    }
                    else
                    {
                        var message = jsonDoc.RootElement.GetProperty("message").GetString();
                        throw new InvalidOperationException($"Altın API başarısız döndü: {message}");
                    }
        }

        public async Task PruneOldGoldPricesAsync()
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            await _context.GoldPrices
                .Where(x => x.Date < oneWeekAgo)
                .ExecuteDeleteAsync();
        }
    }
}
