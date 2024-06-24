using ECommereceApi.DTOs.Order;
using ECommereceApi.DTOs.Product;

namespace ECommereceApi.IRepo
{
    public interface IOrderRepo
    {
        Task<bool> IsAllCartItemsAvailableAsync(CartProductsDTO cartProducts);
        Task<OrderPreviewDTO> GetOrderPreviewAsync(CartProductsDTO cartProductsDTO);
    }
}
