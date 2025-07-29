using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.Json;
using AnatoliaRoot_V2.Models;
using Microsoft.EntityFrameworkCore;
using AnatoliaRoot_V2.Data;
using System.Linq;

namespace AnatoliaRoot_V2.Services
{
    public class ExchangeRateService
    {
        private readonly AppDbContext _context;
        private const string ApiKey = "e9891489cdf80a23f0ea8034031e098b";
        private const string ApiUrl = "https://api.exchangerate.host/live?access_key=" + ApiKey + "&currencies=TRY,EUR,USD";

        public ExchangeRateService(AppDbContext context)
        {
            _context = context;
        }

        public async Task FetchAndSaveRatesAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.GetProperty("success").GetBoolean())
                throw new Exception("Kur API başarısız döndü.");

            var quotes = root.GetProperty("quotes");
            // USD bazlı oranlar: USDTRY, USDEUR
            decimal usdTry = quotes.GetProperty("USDTRY").GetDecimal();
            decimal usdEur = quotes.GetProperty("USDEUR").GetDecimal();
            // EUR bazlı oranı almak için: EURTRY = USDTRY / USDEUR
            decimal eurTry = usdTry / usdEur;
            // 1 TRY = ? USD, 1 TRY = ? EUR
            decimal tryUsd = 1 / usdTry;
            decimal tryEur = 1 / eurTry;

            var rate = new ExchangeRate
            {
                Date = DateTime.UtcNow,
                UsdRate = tryUsd,
                EurRate = tryEur,
                UsdTry = usdTry,
                EurTry = eurTry,
                Source = "exchangerate.host"
            };

            _context.ExchangeRates.Add(rate);
            await _context.SaveChangesAsync();
        }

        public async Task PruneOldRatesAsync()
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            var oldRates = _context.ExchangeRates.Where(x => x.Date < oneWeekAgo);
            _context.ExchangeRates.RemoveRange(oldRates);
            await _context.SaveChangesAsync();
        }
    }
} 