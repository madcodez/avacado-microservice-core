using AutoMapper;
using Avacado.MessageBus;
using Avacado.Services.ShoppingCartAPI.Data;
using Avacado.Services.ShoppingCartAPI.Models;
using Avacado.Services.ShoppingCartAPI.Models.Dto;
using Avacado.Services.ShoppingCartAPI.Services;
using Avacado.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Avacado.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ResponseDto _response;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private IMapper _mapper;

        public CartAPIController(AppDbContext db, IMapper mapper,IProductService productService, ICouponService couponService , IMessageBus 
            messageBus ,IConfiguration configuration)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }
        [HttpPost("ApplyCoupon")]

        public async Task<ResponseDto> ApplyCoupon(CartDto cartDto)

        {
            try
            {
                var cartFromDb = _db.CartHeaders.First(x => x.UserId == cartDto.CartHeader.UserId);

                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;

                _db.CartHeaders.Update(cartFromDb);

                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch(Exception ex) 
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();

            }
            return _response;
        }



        [HttpGet("GetCart/{userId}")]

        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {

                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(x => x.UserId == userId)),
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.Where(x => x.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDto = await _productService.GetProducts();
                 
                foreach (var item in cart.CartDetails) 
                {
                   item.Product = productDto.FirstOrDefault(x => x.Id == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }
                if(!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDto couponDto = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if(couponDto != null  && cart.CartHeader.CartTotal > couponDto.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= couponDto.DiscountAmount;
                        cart.CartHeader.Discount = couponDto.DiscountAmount;
                    }
                    

                }
                _response.Result = cart;
                }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return _response;
            }
            return _response;
        }

        [HttpPost("CartUpsert")]

        public async Task<ResponseDto>  CartUpsert(CartDto cartDto)

        {

            try 
            { 
                var cartHeaderDb =  await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderDb == null) 
                { 
                  //Create a Cart Header
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();


                    //Create a Cart details
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //Check cart details has same product
                    var cartDetailsDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                                                                           u.CartHeaderId == cartHeaderDb.CartHeaderId);
                    if (cartDetailsDb == null) 
                    { 
                        //Create Cart details
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else 
                    {
                        cartDto.CartDetails.First().Count += cartDetailsDb.Count;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsDb.CartDetailsId;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsDb.CartHeaderId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;
            
            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return _response;
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)

        {

            try
            {
                CartDetails cartDetails =  _db.CartDetails.First(u => u.CartDetailsId == cartDetailsId);

                int totalCountCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    
                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
              
                _response.Result = true;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return _response;
            }
            return _response;
        }

        [HttpPost("EmailCartRequest")]

        public async Task<ResponseDto> EmailCartRequest([FromBody]CartDto cartDto)

        {
            try
            {
                await _messageBus.PublishMessage(cartDto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();

            }
            return _response;
        }


    }
}
