using AutoMapper;
using ProductCatalogRepo;
using ProductCatalogRepo.Models;
using ProductCatalogService.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService
{
    public class ProductCatalogService :IProductCatalogService
    {
        private readonly IProductRepo _productRepo;
        private readonly IMapper _mapper;

        public ProductCatalogService(IProductRepo productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;   
        }

        public async Task<ProductDto> AddProductAsync(ProductCreateDto product)
        {
            var model = _mapper.Map<Product>(product);
            var created = await _productRepo.AddProductAsync(model);
            return _mapper.Map<ProductDto?>(created);
        }
            
       

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepo.DeleteProductAsync(id);
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepo.GetProductByIdAsync(id);
            return _mapper.Map<ProductDto?>(product);
        }

        public async Task<IList<ProductDto>> GetProductsAsync()
        {
            var products = await _productRepo.GetProductsAsync(); 
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<ProductDto> UpdateProductAsync(ProductUpdateDto product)
        {
            var model = _mapper.Map<Product>(product);
            var updated = await _productRepo.UpdateProductAsync(model);
            return _mapper.Map<ProductDto>(updated);
        }
    }
}
