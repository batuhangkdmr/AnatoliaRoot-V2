using System;

namespace AnatoliaRoot_V2.Models.ViewModels
{
    public class ExchangeRateViewModel
    {
        public decimal UsdTry { get; set; }
        public decimal EurTry { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
    }
} 