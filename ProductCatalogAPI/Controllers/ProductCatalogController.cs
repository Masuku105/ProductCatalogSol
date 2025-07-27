using Microsoft.AspNetCore.Mvc;
using ProductCatalogService;
using ProductCatalogService.Dtos;
using System.Threading.Tasks;

namespace ProductCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductCatalogController : ControllerBase
    {
        private readonly IProductCatalogService _service;

        public ProductCatalogController(IProductCatalogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetProductsAsync();
            return Ok(products);
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
