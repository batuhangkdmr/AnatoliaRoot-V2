using System;
using System.ComponentModel.DataAnnotations;

namespace AnatoliaRoot_V2.Models
{
    public class GoldPrice
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Altın fiyatları (TL)
        public decimal GramGold { get; set; } // Gram altın
        public decimal QuarterGold { get; set; } // Çeyrek altın
        public decimal HalfGold { get; set; } // Yarım altın

        // API bilgileri
        [StringLength(100)]
        public string Source { get; set; } // API kaynağı

        [StringLength(10)]
        public string Currency { get; set; } = "TRY"; // Para birimi

        [StringLength(50)]
        public string Exchange { get; set; } // Borsa bilgisi

        public long Timestamp { get; set; } // Unix timestamp

        // Değişim bilgileri
        public decimal ChangeRate { get; set; } // Değişim oranı
        public decimal DayHigh { get; set; } // Günlük en yüksek
        public decimal DayLow { get; set; } // Günlük en düşük
        public decimal PrevClose { get; set; } // Önceki kapanış
    }
} 