using System.Collections.Generic;

namespace AnatoliaRoot_V2.Models
{
    public class ProductIndexViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string SortOrder { get; set; }
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
    }
} 