using Avacado.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Avacado.Web.Service.IService
{
    public interface IOrderService
    {
      
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
        Task<ResponseDto?> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto);

        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);

        Task<ResponseDto?> GetAllOrder(string? userId);

        Task<ResponseDto?> GetOrder(int orderId);
        Task<ResponseDto?> UpdateOrderStatus(int orderId,string newStatus);


    }
}
