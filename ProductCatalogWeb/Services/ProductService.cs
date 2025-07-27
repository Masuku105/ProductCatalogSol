using ProductCatalogWeb.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductCatalog.MvcClient.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/products");
                if (!response.IsSuccessStatusCode)
                    return new List<ProductDto>();

                var content = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<IEnumerable<ProductDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return new List<ProductDto>();
            }
        }

        public async Task<ProductUpdateDto> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/{id}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProductUpdateDto>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProductDto> CreateProductAsync(ProductCreateDto createDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(createDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/products", content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProductDto>(responseData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProductDto> UpdateProductAsync(ProductUpdateDto updateDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/products/{updateDto.Id}", content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProductDto>(responseData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/products/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
