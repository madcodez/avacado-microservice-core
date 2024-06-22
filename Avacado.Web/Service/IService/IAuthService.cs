using Avacado.Web.Models;

namespace Avacado.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> RegisterAsync(RegisterRequestDto registerRequestDto);

        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);

        Task<ResponseDto?> AssignRoleAsync(RegisterRequestDto registerRequestDto);
    }
}

