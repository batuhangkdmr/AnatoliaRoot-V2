using System.ComponentModel.DataAnnotations;

namespace AnatoliaRoot_V2.Models
{
    public class CategoryCreateViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
    }
} 