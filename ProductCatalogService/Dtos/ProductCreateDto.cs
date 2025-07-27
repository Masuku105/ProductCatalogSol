using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalogService.Dtos
{
    public class ProductCreateDto
    {
        public string Title { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
    }
}
