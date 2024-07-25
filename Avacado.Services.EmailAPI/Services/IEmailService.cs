using Avacado.Services.EmailAPI.Message;
using Avacado.Services.EmailAPI.Models.Dto;

namespace Avacado.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartLog(CartDto cartDto);
        Task EmailUserRegisterLog(string email);
        Task EmailOrderCreated(RewardsMessage message);
    }
}

