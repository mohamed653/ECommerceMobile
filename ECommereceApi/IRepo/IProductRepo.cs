using ECommereceApi.DTOs.Product;
using ECommereceApi.Models;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IProductRepo
    {
        Task<ProductDisplayDTO> AddProductAsync(ProductAddDTO product);
        Task<Status> DeleteProductAsync(int id);
        Task<bool> IsProductExistsAsync(int id);
        Task<IEnumerable<ProductDisplayDTO>> GetAllProductDisplayDTOsAsync();
        Task<ProductDisplayDTO> GetProductDisplayDTOByIdAsync(int id);
        Task UpdateProductAsync(ProductAddDTO product, int Id);
        Task<IEnumerable<ProductDisplayDTO>> GetAllCategoryProductsAsync(int categoryId);
        //Task<IEnumerable<ProductDisplayDTO>> GetAllProductsForSubCategoryAsync(int subId, string value);
        Task<ProductCategorySubCategoryValuesDTO> GetProductCategorySubCategoryValuesAsync(int productId, int CategoryId, int SubCategoryId);
        Task<List<ProductCategorySubCategoryValuesDTO>> GetAllProductCategorySubCategoryValuesAsync(int productId);
        Task<ProductCategorySubCategoryValuesDTO> AssignValueForProductCategorySubCategory(ProductCategorySubCategoyValueAddDTO input);
        Task<int> DeleteProductCategorySubCategoryValue(int productId, int categoryId, int subCategoryId, string value);
        Task<int> DeleteProductCategorySubCategoryValueAll(int productId, int categoryId, int subCategoryId);
        Task<Status> AddProductPhotosAsync(ProductPictureDTO input);
        Task<List<string>> GetProductPicturesAsync(int ProductId);
        Task RemoveProductPictureAsync(int productId, string picture);
        Task<PagedResult<ProductDisplayDTO>> RenderPaginationForAllProductsAsync(int page, int pageSize);
        Task<PagedResult<ProductDisplayDTO>> RenderSortedPaginationSortedAsync(int page, int pageSize, string sortOrder);
        Task<List<ProductDisplayDTO>> GetAllProductsSearchAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, int? offerId);
        Task<PagedResult<ProductDisplayDTO>> GetAllProductsSearchPaginatedAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, int page, int pageSize, int? offerId);
        Task<bool> IsAllProductsExistsAsync(HashSet<int> productsIds);
        Task<ICollection<ProductDisplayDTO>> GetProductsDisplayDTOsFromCategorySubCategoryIdAndValueAsync(int categorySubCategoryId, string value);
        bool IsOrgignalPriceGreaterThanDiscount(double originalPrice, double? discount);
        Task<bool> IsProductImageExistsAsync(int productId, string imageId);
    }
}
