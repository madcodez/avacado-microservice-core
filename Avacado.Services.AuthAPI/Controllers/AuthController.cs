using Avacado.Services.AuthAPI.Models.Dto;
using Avacado.Services.AuthAPI.Service.IService;
using Avacado.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Avacado.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private IAuthService _authService;
        private ResponseDto _responseDto;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _responseDto = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var errorMessage = await _authService.Register(registerRequestDto);

            if(!string.IsNullOrEmpty(errorMessage)) 
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _authService.Login(loginRequestDto);

            if (loginResponse.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Username or Password is incorrect";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponse;
            return Ok(_responseDto);
        }
        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterRequestDto regRequestDto)
        {
            var assignRoleResponse = await _authService.AssignRole(regRequestDto.Email,regRequestDto.Role.ToUpper());

            if (!assignRoleResponse)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error Encountered";
                return BadRequest(_responseDto);
            }
            
            return Ok(_responseDto);
        }
    }
}
