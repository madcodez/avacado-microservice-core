using Avacado.Services.ShoppingCartAPI.Models.Dto;

namespace Avacado.Services.ShoppingCartAPI.Services.IServices
{
    public interface IProductService
    {
        public  Task<IEnumerable<ProductDto>> GetProducts();
    }
}
