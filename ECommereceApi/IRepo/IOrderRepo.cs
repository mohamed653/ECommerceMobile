using ECommereceApi.DTOs.Product;

namespace ECommereceApi.IRepo
{
    public interface IOrderRepo
    {
        Task<OrderDisplayDTO> GetOrderAsync(int userId);
    }
}
