using ProductCatalogRepo.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogRepo
{
    public class ProductRepo : IProductRepo
    {
        private readonly HttpClient _httpClient;

        public ProductRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;   
        }

        public async Task<Product> AddProductAsync(Product product)
        {

            var response = await _httpClient.PostAsJsonAsync("products", product);
            if (response != null)
            { 
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Product>();
        }
  

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Product>($"products/{id}");
        }

        public async Task<IList<Product>> GetProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Product>>("products") ?? new List<Product>();

        }

        public Task<Product> UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"products/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
