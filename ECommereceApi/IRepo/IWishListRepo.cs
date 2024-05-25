using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IWishListRepo
    {
        Task<List<WishList>> GetWishListByUserId(int userId);
        Task<WishList> AddWishList(WishListDTO wishListDTO);
        Task<int> GetWishListCount(int userId);
        Task<List<Product>>GetTopWishlistedProducts();

        Task<List<WishList>> DeleteWishListItem(int userId, int productId);
        Task<Status> DeleteWishList(int id);
    }
}
