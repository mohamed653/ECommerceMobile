using ECommereceApi.DTOs.Product;
using ECommereceApi.Models;

namespace ECommereceApi.IRepo
{
    public interface ICartRepo
    {
        Task<User> GetUserByIdAsync(int userId);
        bool IsUserHaveProductsInCart(User user);
        Task<CartProductsDTO> GetCartProductsAsync(User user);
        Task DeleteCartItemsAsync(User user);
        Task AddProductsToCartAsync(User user, List<int> productsIds, List<int> amounts);
        Task AddProductToCartAsync(User user, ProductDisplayDTO product, int amount);
    }
}
