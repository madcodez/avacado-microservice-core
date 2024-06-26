using AutoMapper;

using Avacado.Services.ShoppingCartAPI.Models;
using Avacado.Services.ShoppingCartAPI.Models.Dto;

namespace Avacado.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
                
            });      
            return mappingConfig;
        }

    }
}
