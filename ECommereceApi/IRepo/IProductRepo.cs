using ECommereceApi.DTOs;
using ECommereceApi.Models;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IProductRepo
    {
        Task<ProductDisplayDTO> AddProductAsync(ProductAddDTO product);
        Task<Status> DeleteProductAsync(int id);
        Task<IEnumerable<ProductDisplayDTO>> GetAllProductsAsync();
        Task<ProductDisplayDTO> GetProductByIdAsync(int id);
        Task<Status> UpdateProductAsync(ProductAddDTO product, int Id);
        Task<IEnumerable<ProductDisplayDTO>> GetAllCategoryProductsAsync(int categoryId);
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryDTO>> GetAllSubCategoriesForCategoryAsync(int categoryId);
        Task<IEnumerable<ProductDisplayDTO>> GetAllProductsForSubCategoryAsync(int subId, string value);
        Task<Status> AddProductPhotosAsync(ProductPictureDTO input);
        Task<List<string>> GetProductPicturesAsync(int ProductId);
        Task<Status> RemoveProductPictureAsync(int productId, string picture);
        Task<PagedResult<ProductDisplayDTO>> RenderPaginationForAllProductsAsync(int page, int pageSize);
        Task<PagedResult<ProductDisplayDTO>> RenderSortedPaginationSortedAsync(int page, int pageSize, string sortOrder);
        Task<List<ProductDisplayDTO>> GetAllProductsSearchAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds);
        Task<PagedResult<ProductDisplayDTO>> GetAllProductsSearchPaginatedAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, int page, int pageSize);
        Task<bool> IsAllProductsExistsAsync(HashSet<int> productsIds);
    }
}
