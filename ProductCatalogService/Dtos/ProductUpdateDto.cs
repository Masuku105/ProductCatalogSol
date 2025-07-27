using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalogService.Dtos
{
    public class ProductUpdateDto : ProductCreateDto
    {
        public int Id { get; set; }
    }
}
