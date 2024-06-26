﻿using Avacado.Web.Models;
using Avacado.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Avacado.Web.Controllers
{
    public class HomeController : Controller
    {
		private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService , ICartService cartService)
        {
			_productService = productService;
			_cartService = cartService;
		}

		public async Task<IActionResult> Index()
		{
			List<ProductDto>? list = new();

			ResponseDto? response = await _productService.GetAllProductsAsync();

			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
			}
			else
			{
				TempData["error"] = response?.Message;
			}

			return View(list);
		}
		[Authorize]
		public async Task<IActionResult> ProductDetails(int productId)
		{
			ProductDto? model = new();

			ResponseDto? response = await _productService.GetProductByIdAsync(productId);

			if (response != null && response.IsSuccess)
			{
				model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
			}
			else
			{
				TempData["error"] = response?.Message;
			}

			return View(model);
		}
		[HttpPost]
		[Authorize]
		[ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {

			CartDto cartDto = new CartDto()
			{
				CartHeader = new CartHeaderDto
				{
					UserId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
				}
			};

			CartDetailsDto cartDetails = new CartDetailsDto
			{
				Count = productDto.Count,
				ProductId = productDto.Id
			};

			List<CartDetailsDto> cartDetailsDtos = new List<CartDetailsDto> { cartDetails };

			cartDto.CartDetails = cartDetailsDtos;

            ResponseDto? response = await _cartService.CartUpsertAsync(cartDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Item has been added to the Cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(productDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}