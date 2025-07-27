using AutoMapper;
using ProductCatalogRepo.Models;
using ProductCatalogService.Dtos;

namespace ProductCatalogService
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, ProductCreateDto>().ReverseMap();
            CreateMap<Product, ProductUpdateDto>().ReverseMap();
        }
        
    }
}
