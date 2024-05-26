using ECommereceApi.DTOs.Product;
using ECommereceApi.Models;

namespace ECommereceApi.IRepo
{
    public interface ICartRepo
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<CartProductsDTO> GetCartProductsAsync(User user);
        Task DeleteCartItemsAsync(User user);
        Task AddProductToCartAsync(User user, ProductDisplayDTO product, int amount);
        Task ClearAndAddMultipleProducts(User user, List<int> productsIds, List<int> amounts);
    }
}
