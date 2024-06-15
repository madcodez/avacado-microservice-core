using Avacado.Web.Models;
using Avacado.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace Avacado.Web.Controllers
{
    public class CouponController : Controller
    {
        private ICouponService _couponService;
        public CouponController(ICouponService couponService) 
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto> ?  list = new();

            ResponseDto? response = await _couponService.GetAllCouponsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            else 
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }
        public async Task<IActionResult> CreateCoupon()
        {
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponDto model)
        {
            if (ModelState.IsValid)
            {
               

                ResponseDto? response = await _couponService.CreateCouponAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Created Successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
           
            return View(model);
        }

        public async Task<IActionResult> DeleteCoupon(int couponId)
        {
			ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);

			if (response != null && response.IsSuccess)
			{
				CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                return View(model);
			}
            else
            {
                TempData["error"] = response?.Message;
            }

            return View();
        }
        [HttpPost]
		public async Task<IActionResult> DeleteCoupon(CouponDto couponDto)
		{
			ResponseDto? response = await _couponService.DeleteCouponAsync(couponDto.Id);

			if (response != null && response.IsSuccess)
			{
                TempData["success"] = "Deleted Successfully";
                return RedirectToAction(nameof(CouponIndex));
			}
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(couponDto);
		}
	}
}
