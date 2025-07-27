using ProductCatalogRepo.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
            if (id == 0)
            {
                return null;    
            }
            var response = await _httpClient.GetAsync($"products/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Product>(contentStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<IList<Product>> GetProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Product>>("products") ?? new List<Product>();

        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var response = await _httpClient.PutAsJsonAsync($"products/{product.Id}", product);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<Product>();
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"products/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
