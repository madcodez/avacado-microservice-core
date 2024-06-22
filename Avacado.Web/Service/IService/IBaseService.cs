
using Avacado.Web.Models;

namespace Avacado.Web.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto , bool withBearer = true);
    }
}
