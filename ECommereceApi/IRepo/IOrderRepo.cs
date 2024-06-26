using ECommereceApi.DTOs.Order;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Enums;

namespace ECommereceApi.IRepo
{
    public interface IOrderRepo
    {
        Task<bool> IsAllCartItemsAvailableAsync(CartProductsDTO cartProducts);
        Task<OrderPreviewDTO> GetOrderPreviewAsync(CartProductsDTO cartProductsDTO);
        Task<OrderPreviewWithoutOffersDTO> AddOrderWithoutOfferAsync(CartProductsDTO cartProductsDTO, AddOrderWithoutOfferDTO addOrderWithoutOfferDTO);
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task ChangeOrderStatusAsync(Guid orderId, OrderStatus newStatus);
        Task<PagedResult<OrderDisplayDTO>> GetUserOrdersPaginatedAsync(int userId, int page, int pageSize);
        Task<double> GetFinalOfferPriceAsync(int offerId, int userId);
    }
}
