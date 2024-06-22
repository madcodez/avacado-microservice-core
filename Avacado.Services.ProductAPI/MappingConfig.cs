using AutoMapper;

using Avacado.Services.ProductAPI.Models;
using Avacado.Services.ProductAPI.Models.Dto;

namespace Avacado.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDto>();
                config.CreateMap<ProductDto, Product>();
            });      
            return mappingConfig;
        }

    }
}
