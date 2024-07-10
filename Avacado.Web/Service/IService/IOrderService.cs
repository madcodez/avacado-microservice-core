using Avacado.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Avacado.Web.Service.IService
{
    public interface IOrderService
    {
      
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
  

    }
}
