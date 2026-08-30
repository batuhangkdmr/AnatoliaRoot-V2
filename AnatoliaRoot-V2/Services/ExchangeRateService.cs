using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using AnatoliaRoot_V2.Models;
using Microsoft.EntityFrameworkCore;
using AnatoliaRoot_V2.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace AnatoliaRoot_V2.Services
{
    public class ExchangeRateService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public ExchangeRateService(
            AppDbContext context,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["ExternalApis:ExchangeRate:ApiKey"];
            _apiUrl = configuration["ExternalApis:ExchangeRate:ApiUrl"];
        }

        public async Task FetchAndSaveRatesAsync()
        {
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_apiUrl))
                throw new InvalidOperationException("Döviz API yapılandırması eksik.");

            var httpClient = _httpClientFactory.CreateClient("ExchangeRate");
            var requestUrl = $"{_apiUrl}?access_key={Uri.EscapeDataString(_apiKey)}&currencies=TRY,EUR,USD";
            using var response = await httpClient.GetAsync(requestUrl);
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
            if (usdTry <= 0 || usdEur <= 0)
                throw new InvalidOperationException("Kur API geçersiz veya sıfır oran döndürdü.");

            long? sourceTimestamp = null;
            if (root.TryGetProperty("timestamp", out var timestampElement)
                && timestampElement.TryGetInt64(out var timestamp)
                && timestamp > 0
                && timestamp <= 253402300799)
            {
                sourceTimestamp = timestamp;
            }

            if (sourceTimestamp.HasValue
                && await _context.ExchangeRates.AnyAsync(item => item.SourceTimestamp == sourceTimestamp.Value))
            {
                return;
            }

            // EUR bazlı oranı almak için: EURTRY = USDTRY / USDEUR
            decimal eurTry = usdTry / usdEur;
            // 1 TRY = ? USD, 1 TRY = ? EUR
            decimal tryUsd = 1 / usdTry;
            decimal tryEur = 1 / eurTry;

            var rate = new ExchangeRate
            {
                Date = sourceTimestamp.HasValue
                    ? DateTimeOffset.FromUnixTimeSeconds(sourceTimestamp.Value).UtcDateTime
                    : DateTime.UtcNow,
                SourceTimestamp = sourceTimestamp,
                UsdRate = tryUsd,
                EurRate = tryEur,
                UsdTry = usdTry,
                EurTry = eurTry,
                Source = "exchangerate.host"
            };

            _context.ExchangeRates.Add(rate);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) when (sourceTimestamp.HasValue)
            {
                _context.Entry(rate).State = EntityState.Detached;
                if (!await _context.ExchangeRates.AnyAsync(item => item.SourceTimestamp == sourceTimestamp.Value))
                    throw;
            }
        }

        public async Task PruneOldRatesAsync()
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            await _context.ExchangeRates
                .Where(x => x.Date < oneWeekAgo)
                .ExecuteDeleteAsync();
        }
    }
}
