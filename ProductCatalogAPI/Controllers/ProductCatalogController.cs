using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ProductCatalogService;
using ProductCatalogService.Dtos;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace ProductCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductCatalogController : ControllerBase
    {
        private readonly IProductCatalogService _service;
        private readonly IDistributedCache _cache;

        public ProductCatalogController(IProductCatalogService service, IDistributedCache cache)
        {
            _service = service;
            _cache = cache;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string cacheKey = "products:all";
            string cachedData = null;
            try
            {
                cachedData = await _cache.GetStringAsync(cacheKey);
            }
            catch(Exception ex)
            {
               
            }
           

            if (!string.IsNullOrEmpty(cachedData))
            {
                var products = JsonConvert.DeserializeObject<List<ProductDto>>(cachedData);
                return Ok(products); 
            }

            var freshData = await _service.GetProductsAsync();

            var serialized = JsonConvert.SerializeObject(freshData);
            await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return Ok(freshData); 
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _service.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }          
            return Ok(product);
            
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var created = await _service.AddProductAsync(dto);
            if(created == null)
            {
                return BadRequest();
            }
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var updated = await _service.UpdateProductAsync(dto);
            if (updated == null)
            {
                return NotFound();
            }           
            return Ok(updated);
            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteProductAsync(id);
            if(!deleted)
            {
                return NotFound();
            }
            return Ok(deleted); 
        }
    }
}
