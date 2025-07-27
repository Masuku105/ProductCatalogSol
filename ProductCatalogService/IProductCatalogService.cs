using ProductCatalogRepo.Models;
using ProductCatalogService.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService
{
    public interface IProductCatalogService
    {

        Task<IList<ProductDto>> GetProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ProductDto> AddProductAsync(ProductCreateDto product);
        Task<ProductDto> UpdateProductAsync(ProductUpdateDto product);
        Task<bool> DeleteProductAsync(int id);
    }
}
