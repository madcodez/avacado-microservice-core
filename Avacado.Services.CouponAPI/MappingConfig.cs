using AutoMapper;
using Avacado.Services.CouponAPI.Models;
using Avacado.Services.CouponAPI.Models.Dto;

namespace Avacado.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Coupon, CouponDto>();
                config.CreateMap<CouponDto, Coupon>();
            });      
            return mappingConfig;
        }

    }
}
