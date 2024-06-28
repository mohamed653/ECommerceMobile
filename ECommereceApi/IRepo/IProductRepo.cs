using ECommereceApi.DTOs.Product;
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
        Task<CategoryDTO> AddCategoryAsync(CategoryAddDTO category);
        Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryAddDTO category);
        Task<Status> DeleteCategoryAsync(int categoryId);
        Task<List<SubCategoryDTO>> GetAllSubCategoriesAsync();
        Task<SubCategoryDTO> GetSubCategoryById(int id);
        Task<SubCategoryDTO> UpdateSubCategoryAsync(int subId, SubCategoryAddDTO subcat);
        Task<Status> DeleteSubCategoryAsync(int id);
        Task<SubCategoryDTO> AddSubCategoryAsync(SubCategoryAddDTO category);
        Task<IEnumerable<SubCategoryDTO>> GetAllSubCategoriesForCategoryAsync(int categoryId);
        //Task<IEnumerable<ProductDisplayDTO>> GetAllProductsForSubCategoryAsync(int subId, string value);
        Task<ProductCategorySubCategoryValuesDTO> GetProductCategorySubCategoryValuesAsync(int productId, int CategoryId, int SubCategoryId);
        Task<List<ProductCategorySubCategoryValuesDTO>> GetAllProductCategorySubCategoryValuesAsync(int productId);
        Task<ProductCategorySubCategoryValuesDTO> AssignValueForProductCategorySubCategory(ProductCategorySubCategoyValueAddDTO input);
        Task<int> DeleteProductCategorySubCategoryValue(int productId, int categoryId, int subCategoryId, string value);
        Task<int> DeleteProductCategorySubCategoryValueAll(int productId, int categoryId, int subCategoryId);
        Task<Status> AddProductPhotosAsync(ProductPictureDTO input);
        Task<List<string>> GetProductPicturesAsync(int ProductId);
        Task<Status> RemoveProductPictureAsync(int productId, string picture);
        Task<PagedResult<ProductDisplayDTO>> RenderPaginationForAllProductsAsync(int page, int pageSize);
        Task<PagedResult<ProductDisplayDTO>> RenderSortedPaginationSortedAsync(int page, int pageSize, string sortOrder);
        Task<List<ProductDisplayDTO>> GetAllProductsSearchAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds);
        Task<PagedResult<ProductDisplayDTO>> GetAllProductsSearchPaginatedAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, int page, int pageSize);
        Task<bool> IsAllProductsExistsAsync(HashSet<int> productsIds);
        Task<CategorySubCategoryValueDTO> AddSubCategoryValueAsync(CategorySubCategoryValuesAddDTO input);
        Task<int> AssignSubCategoryToCategoryAsync(int categoryId, int subCategoryId);
        Task<ICollection<SubCategoriesValuesForCategoryDTO>> GetAllCategoriesDetailsAsync();
        Task<SubCategoriesValuesForCategoryDTO> GetCategoryDetailsAsync(int categoryId);
        Task<CategoriesValuesForSubCategoryDTO> GetSubCategoryDetails(int subCategoryId);
        Task<ProductCategorySubCategoryValuesDTO> UpdateCategorySubCategoryValue(CategorySubCategoryValuesAddDTO addDTO, string newValue);
        Task<bool> IsCategorySubCategoryExistsAsync(int categoryId, int subCategoryId);
        Task<bool> IsCategoryExistsAsync(int id);
        Task<bool> IsSubCategoryExistsAsync(int id);
        Task<int> GetCategorySubCategoryIdFromSeparateIds(int categoryId, int subCategoryId);
        Task<ICollection<ProductDisplayDTO>> GetProductsDisplayDTOsFromCategorySubCategoryIdAndValueAsync(int categorySubCategoryId, string value);
    }
}
