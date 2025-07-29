using System;
using System.ComponentModel.DataAnnotations;

namespace AnatoliaRoot_V2.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; } // Kurun alındığı tarih (saat dahil)

        [Required]
        public decimal UsdRate { get; set; } // 1 TRY = ? USD

        [Required]
        public decimal EurRate { get; set; } // 1 TRY = ? EUR

        [Required]
        public decimal UsdTry { get; set; } // 1 USD = ? TRY

        [Required]
        public decimal EurTry { get; set; } // 1 EUR = ? TRY

        [StringLength(100)]
        public string Source { get; set; } // API kaynağı (örn: exchangerate.host)
    }
} 