using Avacado.Services.AuthAPI.Models;

namespace Avacado.Services.AuthAPI.Service.IService
{
    public interface IJwtGeneratorService
    {
        string GenerateToken(ApplicationUser applicationUser);
    }
}
