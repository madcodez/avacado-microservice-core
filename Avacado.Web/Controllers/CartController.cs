using Avacado.Web.Models;
using Avacado.Web.Service.IService;
using Avacado.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;

namespace Avacado.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly ICouponService _couponService;

        public CartController(ICartService cartService, IOrderService orderService,ICouponService couponService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _couponService = couponService;   
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDefaultBasedOnUser());
        }
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDefaultBasedOnUser());
        }
        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            CartDto cart = await LoadCartDefaultBasedOnUser();



            cart.CartHeader.Name = cartDto.CartHeader.Name;
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;

            ResponseDto? response  = await _orderService.CreateOrderAsync(cart);
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

            if(response.Result != null && response.IsSuccess) 
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                //get stripe instancs
                StripeRequestDto stripeRequestDto = new()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDto.OrderHeaderId,
                    CancelUrl = domain + "cart/checkout",
                    OrderHeader = orderHeaderDto


                };
              var stripeResponse = await _orderService.CreateStripeSessionAsync(stripeRequestDto);
              StripeRequestDto stripe= JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Result));
              Response.Headers.Add("Location", stripe.StripeSessionUrl);
              return new StatusCodeResult(303);
            }
            return View();
        }
        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDto? response = await _orderService.ValidateStripeSession(orderId); 
            if (response != null & response.IsSuccess)
            {

                OrderHeaderDto orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
                if (orderHeader.Status == SD.Status_Approved)
                {
                    return View(orderId);
                }
            }
            //redirect to some error page based on status
            return View(orderId);
        }

        public async Task<CartDto> LoadCartDefaultBasedOnUser()
        {
            var userId = User.Claims.Where(x =>x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);

            if(response != null & response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDto();


        }
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveCartAsync(cartDetailsId);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = "Not Active Coupon";
                
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            CartDto cart = await LoadCartDefaultBasedOnUser();

            var couponCode = cartDto.CartHeader.CouponCode;

            ResponseDto responseCode = await _couponService.GetCouponAsync(couponCode);
            CouponDto couponDto = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(responseCode.Result));
            
            if (string.IsNullOrEmpty(couponDto?.CouponCode) || couponDto?.MinAmount > cart.CartHeader.CartTotal) 
            {
                TempData["error"] = "Enter a valid coupon code";
                return RedirectToAction(nameof(CartIndex));

            }
            else 
            {
                ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
                if (response != null & response.IsSuccess)
                {
                    TempData["success"] = "Cart updated successfully";
                    return RedirectToAction(nameof(CartIndex));
                }

            }

          
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDefaultBasedOnUser();
            cart.CartHeader.Email = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.EmailCartAsync(cart);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Email will be processed amd sent shortly";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";

            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

    }
}
