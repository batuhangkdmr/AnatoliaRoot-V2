using System.Collections.Generic;

namespace AnatoliaRoot_V2.Models
{
    public class ProductIndexViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
} 