using Microsoft.AspNetCore.Mvc;
using Avacado.Services.OrderAPI.Data;
using Avacado.Services.OrderAPI.Models.Dto;
using Avacado.Services.OrderAPI.Services.IServices;
using Avacado.MessageBus;
using AutoMapper;
using Avacado.Services.OrderAPI.Models;
using Avacado.Services.OrderAPI.Utility.SD;

namespace Avacado.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ResponseDto _response;
        private readonly IProductService _productService;
        
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private IMapper _mapper;

        public OrderAPIController(AppDbContext db, IMapper mapper, IProductService productService, IMessageBus
            messageBus, IConfiguration configuration)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
            _productService = productService;
            _messageBus = messageBus;
            _configuration = configuration;
        }
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody]CartDto cartDto)
        {

            try
            {
                OrderHeaderDto orderHeaderDto =_mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderHeader = _db.OrderHeader.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderHeader.OrderHeaderId;
                _response.Result = orderHeaderDto;
            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.Message= ex.Message;

            }
            return _response;
        }
    }
}
