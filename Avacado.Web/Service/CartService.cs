using Avacado.Web.Models;
using Avacado.Web.Service.IService;
using Avacado.Web.Utility;

namespace Avacado.Web.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService) 
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.CartApiBase + "/api/cart/ApplyCoupon"
            });
        }

        public async Task<ResponseDto?> CartUpsertAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.CartApiBase + "/api/cart/CartUpsert"
            });
        }

     

      

     

        public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
        {

            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CartApiBase + "/api/cart/GetCart/" + userId
            });
        }

     

        public async Task<ResponseDto?> RemoveCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDetailsId,
                Url = SD.CartApiBase + "/api/cart/RemoveCart"
            });
        }

     
    }
}
