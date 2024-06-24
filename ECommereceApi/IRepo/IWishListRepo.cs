using ECommereceApi.DTOs.Product;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IWishListRepo
    {
        Task<List<WishList>> GetWishListByUserId(int userId);
        Task<Status> AddWishList(WishListDTO wishListDTO);
        Task<int> GetWishListCount(int userId);
        Task<List<ProductDisplayDTO>> GetWishListProducts(int userId);
        Task<List<ProductDisplayDTO>> GetTopWishlistedProducts();

        Task<List<WishList>> DeleteWishListItem(int userId, int productId);
        Task<Status> DeleteWishList(int id);
    }
}
