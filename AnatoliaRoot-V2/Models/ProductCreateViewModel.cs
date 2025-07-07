using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AnatoliaRoot_V2.Models
{
    public class ProductCreateViewModel: IValidatableObject
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public int? AnaCategoryId { get; set; }

        [Required]
        public int? AltCategoryId { get; set; }

        public IFormFile ImageFile { get; set; }

        public string ExistingImageUrl { get; set; }

        // Dropdownlar için
        [BindNever]
        public IEnumerable<Category> AnaKategoriler { get; set; }
        [BindNever]
        public IEnumerable<Category> AltKategoriler { get; set; }
        
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Eğer hem ImageFile hem de ExistingImageUrl null ise, hata ver.
        if (ImageFile == null && string.IsNullOrEmpty(ExistingImageUrl))
        {
            yield return new ValidationResult("Resim dosyası yüklenmeli.", new[] { nameof(ImageFile) });
        }
    }
    }
} 