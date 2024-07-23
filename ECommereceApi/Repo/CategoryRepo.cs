using AutoMapper;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly ECommerceContext _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IFileCloudService _fileCloudService;
        public CategoryRepo(ECommerceContext db, IMapper mapper, IWebHostEnvironment env, IFileCloudService fileCloudService)
        {
            _db = db;
            _mapper = mapper;
            _env = env;
            _fileCloudService = fileCloudService;
        }
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            return _mapper.Map<List<CategoryDTO>>(await _db.Categories.Include(c => c.CategorySubCategory).ThenInclude(s => s.SubCategory).ToListAsync());
        }
        public async Task<CategoryDTO> AddCategoryAsync(CategoryAddDTO category)
        {
            var result = await _db.Categories.AddAsync(new Category() { Name = category.Name });
            if (category.Image is not null)
            {
                //var imageresult = await UploadImage(category.Image);
                var imageresult = await _fileCloudService.UploadImagesAsync(category.Image);
                result.Entity.ImageId = imageresult;
                result.Entity.ImageUri = _fileCloudService.GetImageUrl(imageresult);
            }
            foreach (var subId in category.SubCategoriesIds)
            {
                result.Entity.CategorySubCategory.Add(new CategorySubCategory() { CategoryId = result.Entity.CategoryId, SubCategoryId = subId });
            }
            await MySaveChangesAsync();
            return _mapper.Map<CategoryDTO>(result.Entity);
        }
        public async Task MySaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
        public async Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryAddDTO category)
        {
            var target = await _db.Categories.Include(c => c.CategorySubCategory).ThenInclude(sc => sc.SubCategory).FirstOrDefaultAsync(c => c.CategoryId == id);
            target.Name = category.Name;
            target.CategorySubCategory.Clear();
            foreach (var subId in category.SubCategoriesIds)
            {
                target.CategorySubCategory.Add(new CategorySubCategory() { CategoryId = target.CategoryId, SubCategoryId = subId });
            }
            if (category.Image is not null)
            {
                var imageId = target.ImageId;
                if (imageId is not null)
                    _fileCloudService.DeleteImageAsync(imageId);
                //var imageresult = await UploadImage(category.Image);
                var imageresult = await _fileCloudService.UploadImagesAsync(category.Image);
                target.ImageId = imageresult;
            }
            _db.Update(target);
            await MySaveChangesAsync();
            return _mapper.Map<CategoryDTO>(target);
        }
        public async Task<Status> DeleteCategoryAsync(int categoryId)
        {
            var target = await _db.Categories.Include(c => c.CategorySubCategory).ThenInclude(sc => sc.SubCategory).FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            if (target is null)
                return Status.NotFound;
            target.CategorySubCategory.Clear();
            if (target.ImageId is not null)
                _fileCloudService.DeleteImageAsync(target.ImageId);
            _db.Categories.Remove(target);
            await MySaveChangesAsync();
            return Status.Success;
        }
        public async Task<List<SubCategoryDTO>> GetAllSubCategoriesAsync()
        {
            return _mapper.Map<List<SubCategoryDTO>>(await _db.SubCategories.ToListAsync());
        }
        public async Task<SubCategoryDTO> GetSubCategoryById(int id)
        {
            return _mapper.Map<SubCategoryDTO>(await _db.SubCategories.Include(s => s.CategorySubCategories).FirstOrDefaultAsync(s => s.SubCategoryId == id));
        }
        public async Task<SubCategoryDTO> UpdateSubCategoryAsync(int subId, SubCategoryAddDTO subcat)
        {
            var target = await _db.SubCategories.FirstOrDefaultAsync(s => s.SubCategoryId == subId);
            if (target is null)
                return null;
            target.Name = subcat.Name;
            _db.Update(target);
            await MySaveChangesAsync();
            return _mapper.Map<SubCategoryDTO>(target);
        }
        public async Task<Status> DeleteSubCategoryAsync(int id)
        {
            var target = await _db.SubCategories.Include(s => s.CategorySubCategories).FirstOrDefaultAsync(s => s.SubCategoryId == id);
            if (target is null)
                return Status.NotFound;
            //if(target.Categories.Any() || target.ProductSubCategories.Any())
            //    return Status.Failed;
            _db.SubCategories.Remove(target);
            await MySaveChangesAsync();
            return Status.Success;
        }
        public async Task<SubCategoryDTO> AddSubCategoryAsync(SubCategoryAddDTO category)
        {
            var Subcategory = _mapper.Map<SubCategory>(category);
            var result = await _db.SubCategories.AddAsync(Subcategory);
            await MySaveChangesAsync();
            return _mapper.Map<SubCategoryDTO>(result.Entity);
        }
        public async Task<IEnumerable<SubCategoryDTO>> GetAllSubCategoriesForCategoryAsync(int categoryId)
        {
            return _mapper.Map<List<SubCategoryDTO>>(await _db.SubCategories.Where(s => s.CategorySubCategories.Select(c => c.CategoryId).Contains(categoryId)).ToListAsync());
        }
        public async Task<CategorySubCategoryValueDTO> AddSubCategoryValueAsync(CategorySubCategoryValuesAddDTO input)
        {
            var result = await _db.CategorySubCategoryValues.AddAsync(new CategorySubCategoryValues()
            {
                CategorySubCategoryId = await GetCategorySubCategoryIdFromSeparateIds(input.CategoryId, input.SubCategoryId),
                Value = input.Value
            });
            if (input.Image is not null)
            {
                var imageresult = await _fileCloudService.UploadImagesAsync(input.Image);
                result.Entity.ImageId = imageresult;
                result.Entity.ImageUri = _fileCloudService.GetImageUrl(imageresult);
            }
            var output = _mapper.Map<CategorySubCategoryValueDTO>(input);
            output.ImageId = result.Entity?.ImageId;
            output.ImageUrl = result.Entity?.ImageUri;

            await MySaveChangesAsync();
            output.Id = result.Entity.Id;
            return output;
        }
        public async Task<int> AssignSubCategoryToCategoryAsync(int categoryId, int subCategoryId)
        {
            var result = await _db.CategorySubCategory.AddAsync(new CategorySubCategory() { CategoryId = categoryId, SubCategoryId = subCategoryId });
            await MySaveChangesAsync();
            return result.Entity.CategorySubCategoryId;
        }
        public async Task<ICollection<SubCategoriesValuesForCategoryDTO>> GetAllCategoriesDetailsAsync()
        {
            var CategoriesIds = await GetAllCategoriesIdsAsync();
            return await GetAllCategoriesDetailsFromIdsAsync(CategoriesIds);
        }
        public async Task<SubCategoriesValuesForCategoryDTO> GetCategoryDetailsAsync(int categoryId)
        {
            var category = await GetCategoryWithSubCategoryWithValuesAsync(categoryId);
            return await MapSubCategoriesValuesForCategory(category);
        }
        public async Task<CategoriesValuesForSubCategoryDTO> GetSubCategoryDetails(int subCategoryId)
        {
            var subCategory = await GetSubCategoryWithCategoryValuesAsync(subCategoryId);
            return await MapCategoriesValuesForSubCategory(subCategory);
        }
        public async Task<ProductCategorySubCategoryValuesDTO> UpdateCategorySubCategoryValue(CategorySubCategoryValuesAddDTO addDTO, string newValue)
        {
            var CategorySubCategory = await _db.CategorySubCategory
                .FirstOrDefaultAsync(c => c.CategoryId == addDTO.CategoryId && c.SubCategoryId == addDTO.SubCategoryId);
            if (CategorySubCategory is null)
                return null;
            int CategorySubCategoryId = CategorySubCategory.CategorySubCategoryId;
            var target = await _db.CategorySubCategoryValues
                .FirstOrDefaultAsync(c => c.CategorySubCategoryId == CategorySubCategoryId && c.Value.ToLower() == addDTO.Value.ToLower());
            var CategorySubCategoryValueId = GetCategorySubCategoryValueIdAsync(addDTO.CategoryId, addDTO.SubCategoryId, addDTO.Value);
            if (target is null)
                return null;
            target.Value = newValue;

            if (addDTO.Image is not null)
            {
                if (target.ImageId != null)
                {
                    var output = await _fileCloudService.UpdateImageAsync(addDTO.Image, target.ImageId);
                    target.ImageId = output;
                    target.ImageUri = _fileCloudService.GetImageUrl(output);
                }
                else
                {
                    var output = await _fileCloudService.UploadImagesAsync(addDTO.Image);
                    target.ImageId = output;
                    target.ImageUri = _fileCloudService.GetImageUrl(output);

                }
            }
            else
            {
                if (target.ImageId != null)
                {
                    var result = await _fileCloudService.DeleteImageAsync(target.ImageId);
                    if (result)
                    {
                        target.ImageId = null;
                        target.ImageUri = null;
                    }
                }
            }
            _db.Update(target);
            await MySaveChangesAsync();
            await _db.Entry(target).Reference(t => t.CategorySubCategory).TargetEntry.Reference(te => te.Category).LoadAsync();
            await _db.Entry(target).Reference(t => t.CategorySubCategory).TargetEntry.Reference(te => te.SubCategory).LoadAsync();
            return _mapper.Map<ProductCategorySubCategoryValuesDTO>(target);
        }
        public async Task<bool> IsCategorySubCategoryExistsAsync(int categoryId, int subCategoryId)
        {
            return await _db.CategorySubCategory.AnyAsync(cs => cs.CategoryId == categoryId && cs.SubCategoryId == subCategoryId);
        }
        public async Task<bool> IsCategoryExistsAsync(int id)
        {
            return await _db.Categories.AnyAsync(c => c.CategoryId == id);
        }
        public async Task<bool> IsSubCategoryExistsAsync(int id)
        {
            return await _db.SubCategories.AnyAsync(s => s.SubCategoryId == id);
        }
        public Task<int> GetCategorySubCategoryIdFromSeparateIds(int categoryId, int subCategoryId)
        {
            return _db.CategorySubCategory.Where(cs => cs.CategoryId == categoryId && cs.SubCategoryId == subCategoryId).Select(cs => cs.CategorySubCategoryId).FirstOrDefaultAsync();
        }
        public async Task<ICollection<int>> GetAllCategoriesIdsAsync()
        {
            return await _db.Categories.Select(c => c.CategoryId).ToListAsync();
        }
        public async Task<int> GetCategorySubCategoryValueIdAsync(int categoryId, int subCategoryId, string value)
        {
            var output = await _db.CategorySubCategoryValues
                .FirstOrDefaultAsync(c => c.CategorySubCategory.CategoryId == categoryId && c.CategorySubCategory.SubCategoryId == subCategoryId && c.Value.ToLower() == value.ToLower());
            if (output is null)
                return -1;
            return output.Id;
        }
        public async Task<ICollection<SubCategoriesValuesForCategoryDTO>> GetAllCategoriesDetailsFromIdsAsync(ICollection<int> ids)
        {
            List<SubCategoriesValuesForCategoryDTO> output = new();
            foreach (var id in ids)
            {
                output.Add(await GetCategoryDetailsAsync(id));
            }
            return output;
        }
        public async Task<CategoriesValuesForSubCategoryDTO> MapCategoriesValuesForSubCategory(SubCategory subCategory)
        {
            var categories = await GetCategoriesForSubCategoryAsync(subCategory);
            var output = _mapper.Map<CategoriesValuesForSubCategoryDTO>(subCategory);
            output.Categories = _mapper.Map<List<CategoryValuesDTO>>(categories);
            foreach (var category in output.Categories)
            {
                var categorySubCategoryId = await GetCategorySubCategoryIdFromSeparateIds(category.CategoryId, subCategory.SubCategoryId);
                var categoryValues = _db.CategorySubCategoryValues.Where(sc => sc.CategorySubCategoryId == categorySubCategoryId);
                category.Values = _mapper.Map<ICollection<SubCategoryValuesDetailsDTO>>(categoryValues);
            }
            return output;
        }
        public async Task<Category> GetCategoryWithSubCategoryWithValuesAsync(int categoryId)
        {
            return await _db.Categories
                .Include(c => c.CategorySubCategory).ThenInclude(c => c.SubCategory)
                .Include(c => c.CategorySubCategory).ThenInclude(c => c.CategorySubCategoryValues)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }
        public async Task<SubCategory> GetSubCategoryWithCategoryValuesAsync(int subCategoryId)
        {
            return await _db.SubCategories
                .Include(sc => sc.CategorySubCategories).ThenInclude(c => c.Category)
                .Include(c => c.CategorySubCategories).ThenInclude(c => c.CategorySubCategoryValues)
                .FirstOrDefaultAsync(c => c.SubCategoryId == subCategoryId);
        }
        public async Task<ICollection<Category>> GetCategoriesForSubCategoryAsync(SubCategory subCategory)
        {
            return subCategory.CategorySubCategories.Select(c => c.Category).ToList();
        }
        public async Task<SubCategoriesValuesForCategoryDTO> MapSubCategoriesValuesForCategory(Category category)
        {
            var subCategories = await GetSubCategoriesForCategoryAsync(category);
            var output = _mapper.Map<SubCategoriesValuesForCategoryDTO>(category);
            output.SubCategories = _mapper.Map<List<SubCategoryValuesDTO>>(subCategories);
            foreach (var subCategory in output.SubCategories)
            {
                var categorySubCategoryId = await GetCategorySubCategoryIdFromSeparateIds(category.CategoryId, subCategory.SubCategoryId);
                var subCategoryValues = _db.CategorySubCategoryValues.Where(c => c.CategorySubCategoryId == categorySubCategoryId);
                subCategory.Values = _mapper.Map<ICollection<SubCategoryValuesDetailsDTO>>(subCategoryValues);
            }
            return output;
        }
        public async Task<ICollection<SubCategory>> GetSubCategoriesForCategoryAsync(Category category)
        {
            return category.CategorySubCategory.Select(c => c.SubCategory).ToList();
        }
    }
}
