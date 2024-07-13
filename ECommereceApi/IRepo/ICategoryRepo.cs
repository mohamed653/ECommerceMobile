using ECommereceApi.DTOs.Product;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface ICategoryRepo
    {
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
        //=====================================================

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
        Task<int> GetCategorySubCategoryValueIdAsync(int categoryId, int subCategoryId, string value);
    }
}
