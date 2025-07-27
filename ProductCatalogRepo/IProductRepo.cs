using ProductCatalogRepo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogRepo
{
    public interface IProductRepo
    {
       Task<IList<Product>> GetProductsAsync();
       Task<Product> GetProductByIdAsync(int id);
       Task<Product> AddProductAsync(Product product);
       Task<Product> UpdateProductAsync(Product product);
       Task<bool> DeleteProductAsync(int id);  
    }
}
