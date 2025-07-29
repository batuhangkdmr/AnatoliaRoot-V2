using System;

namespace AnatoliaRoot_V2.Models.ViewModels
{
    public class GoldPriceViewModel
    {
        // Altın fiyatları (TL)
        public decimal GramGold { get; set; }
        public decimal QuarterGold { get; set; }
        public decimal HalfGold { get; set; }
        
        // Değişim bilgileri
        public decimal ChangeRate { get; set; }
        public decimal DayHigh { get; set; }
        public decimal DayLow { get; set; }
        public decimal PrevClose { get; set; }

        public DateTime Date { get; set; }
        public string Source { get; set; }
        public string Currency { get; set; }
        public string Exchange { get; set; }
    }
} 