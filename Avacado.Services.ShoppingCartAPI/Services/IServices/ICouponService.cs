using Avacado.Services.ShoppingCartAPI.Models.Dto;

namespace Avacado.Services.ShoppingCartAPI.Services.IServices
{
    public interface ICouponService
    {
        public  Task<CouponDto> GetCoupon(string couponCode);
    }
}
