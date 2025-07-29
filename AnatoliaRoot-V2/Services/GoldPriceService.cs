using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using AnatoliaRoot_V2.Models;
using Microsoft.EntityFrameworkCore;
using AnatoliaRoot_V2.Data;

namespace AnatoliaRoot_V2.Services
{
    public class GoldPriceService
    {
        private readonly AppDbContext _context;
        private const string ApiKey = "997ee36ba7msh7202498499c93a7p100757jsn8b83ebe4f6be";
        private const string ApiUrl = "https://harem-altin-anlik-altin-fiyatlari-live-rates-gold.p.rapidapi.com/economy/live-exchange-rates";

        public GoldPriceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task FetchAndSaveGoldPricesAsync()
        {
            try
            {
                Console.WriteLine("Altın fiyatları çekiliyor...");
                
                using (var client = new HttpClient())
                {
                    // Harem Altın API için gerekli header'ları ekle
                    client.DefaultRequestHeaders.Add("X-RapidAPI-Key", ApiKey);
                    client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "harem-altin-anlik-altin-fiyatlari-live-rates-gold.p.rapidapi.com");

                    // Query parametreleri ekle
                    var url = $"{ApiUrl}?type=gold&code=gramaltin,CEYREKALTIN,YARIMALTIN,TAMALTIN,ONSALTIN";

                    Console.WriteLine($"API URL: {url}");

                    var response = await client.GetStringAsync(url);
                    Console.WriteLine($"API Response: {response}");

                    var jsonDoc = JsonDocument.Parse(response);

                    // Harem Altın API response'unu parse et
                    if (jsonDoc.RootElement.GetProperty("status").GetString() == "success")
                    {
                        var data = jsonDoc.RootElement.GetProperty("data");
                        
                        decimal gramGold = 0;
                        decimal quarterGold = 0;
                        decimal halfGold = 0;
                        decimal changeRate = 0;
                        decimal dayHigh = 0;
                        decimal dayLow = 0;
                        decimal prevClose = 0;

                        // Her altın türü için fiyatları al
                        foreach (var item in data.EnumerateArray())
                        {
                            var currencyCode = item.GetProperty("currencyCode").GetString();
                            var buy = item.GetProperty("buy").GetDecimal();
                            var sell = item.GetProperty("sell").GetDecimal();
                            var change = item.GetProperty("changeRate").GetDecimal();
                            var high = item.GetProperty("dayHigh").GetDecimal();
                            var low = item.GetProperty("dayLow").GetDecimal();
                            var prev = item.GetProperty("prevClose").GetDecimal();

                            Console.WriteLine($"Altın türü: {currencyCode}, Alış: {buy}, Satış: {sell}");

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

                        var goldPrice = new GoldPrice
                        {
                            Date = DateTime.UtcNow,
                            Timestamp = jsonDoc.RootElement.GetProperty("systemTime").GetInt64(),
                            
                            // Altın fiyatları
                            GramGold = gramGold,
                            QuarterGold = quarterGold,
                            HalfGold = halfGold,
                            
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

                        Console.WriteLine($"Alınan fiyatlar - Gram: {gramGold}, Çeyrek: {quarterGold}, Yarım: {halfGold}");

                        _context.GoldPrices.Add(goldPrice);
                        await _context.SaveChangesAsync();
                        
                        Console.WriteLine("Altın fiyatları başarıyla kaydedildi.");
                    }
                    else
                    {
                        Console.WriteLine("API başarısız döndü.");
                        var message = jsonDoc.RootElement.GetProperty("message").GetString();
                        Console.WriteLine($"API Mesajı: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Altın kurları çekilirken hata: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        public async Task PruneOldGoldPricesAsync()
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            var oldPrices = _context.GoldPrices.Where(x => x.Date < oneWeekAgo);
            _context.GoldPrices.RemoveRange(oldPrices);
            await _context.SaveChangesAsync();
        }
    }
} 