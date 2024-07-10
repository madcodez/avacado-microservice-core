using Avacado.Services.OrderAPI.Models.Dto;

namespace Avacado.Services.OrderAPI.Services.IServices
{
    public interface IProductService
    {
        public  Task<IEnumerable<ProductDto>> GetProducts();
    }
}
