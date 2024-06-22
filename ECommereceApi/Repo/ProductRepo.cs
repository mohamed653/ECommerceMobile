using ECommereceApi.Models;
using ECommereceApi.IRepo;
using ECommereceApi.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Azure;
using ECommereceApi.DTOs.Product;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;

namespace ECommereceApi.Repo
{
    public class ProductRepo : IProductRepo
    {
        private readonly ECommerceContext _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly Cloudinary _cloudinary;
        public ProductRepo(IWebHostEnvironment env, ECommerceContext context, IMapper mapper, Cloudinary cloudinary)
        {
            _env = env;
            _db = context;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }
        public async Task<ProductDisplayDTO> AddProductAsync(ProductAddDTO product)
        {
            var result = await _db.Products.AddAsync(_mapper.Map<Product>(product));
            //if (product.SubCategoriesIds.Count != product.SubCategoriesValues.Count)
            //    return null;
            await _db.SaveChangesAsync();
            //var category = await _db.Categories.Include(c => c.Subs).ThenInclude(sc => sc).FirstOrDefaultAsync(c => c.CategoryId == product.CategoryId);
            ////var subcatsIds = category.Subs.Select(s => s.SubId).ToList();            
            //var subcatsIds = category.Subs.Select(sc => sc.SubCategoryId).ToList();

            //for (int i = 0; i < product.SubCategoriesIds.Count; i++)
            //{
            //    if (subcatsIds.Contains(subcatsIds[i]))
            //    {
            //        //var productSubCats = new ProductSubCategory()
            //        //{
            //        //    ProductId = result.Entity.ProductId,
            //        //    //CategorySubCategoryValuesId = subcatsIds[i],
            //        //    //SubCategoryValue = product.SubCategoriesValues[i]
            //        //};
            //        //await _db.ProductSubCategories.AddAsync(productSubCats);
            //    }
            //}
            //await _db.SaveChangesAsync();
            return await GetProductByIdAsync(result.Entity.ProductId);
        }

        public async Task<Status> DeleteProductAsync(int id)
        {
            var product = await _db.Products.Include(p => p.ProductOffers).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return Status.Failed;
            if (product.ProductOffers.Count != 0)
            {
                return Status.Failed;
            }
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return Status.Success;
        }

        public async Task<IEnumerable<ProductDisplayDTO>> GetAllProductsAsync()
        {
            return _mapper.Map<List<ProductDisplayDTO>>(await _db.Products.Include(p => p.Category).Include(p => p.ProductImages).ToListAsync()); ;
        }

        public async Task<ProductDisplayDTO> GetProductByIdAsync(int id)
        {
            return _mapper.Map<ProductDisplayDTO>(await _db.Products.Include(p => p.Category).Include(p => p.ProductImages).SingleOrDefaultAsync(p => p.ProductId == id));
        }

        public async Task<Status> UpdateProductAsync(ProductAddDTO product, int Id)
        {
            var target = await _db.Products.SingleOrDefaultAsync(p => p.ProductId == Id);
            if (target == null)
                return Status.NotFound;
            _mapper.Map(product, target);
            _db.Products.Update(target);
            await _db.SaveChangesAsync();
            return Status.Success;
        }
        public async Task<IEnumerable<ProductDisplayDTO>> GetAllCategoryProductsAsync(int categoryId)
        {
            return _mapper.Map<List<ProductDisplayDTO>>(await _db.Products.Where(p => p.CategoryId == categoryId).Include(p => p.Category).Include(p => p.ProductImages).ToListAsync());
        }
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            return _mapper.Map<List<CategoryDTO>>(await _db.Categories.Include(c => c.Subs).ThenInclude(s => s.SubCategory).ToListAsync());
        }
        public async Task<CategoryDTO> AddCategoryAsync(CategoryAddDTO category)
        {
            var result = await _db.Categories.AddAsync(new Category() { Name = category.Name });
            if (category.Image is not null)
            {
                var imageresult = await UploadImage(category.Image);
                if (imageresult.Error is null)
                {
                    result.Entity.ImageUri = imageresult?.Uri.ToString();
                    result.Entity.ImageId = imageresult.PublicId;
                }
            }
            foreach (var subId in category.SubCategoriesIds)
            {
                result.Entity.Subs.Add(new CategorySubCategory() { CategoryId = result.Entity.CategoryId, SubCategoryId = subId });
            }
            await _db.SaveChangesAsync();
            return _mapper.Map<CategoryDTO>(result.Entity);
        }
        public async Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryAddDTO category)
        {
            var target = await _db.Categories.Include(c => c.Subs).ThenInclude(sc => sc.SubCategory).FirstOrDefaultAsync(c => c.CategoryId == id);
            if (target is null)
                return null;
            target.Name = category.Name;
            target.Subs.Clear();
            foreach (var subId in category.SubCategoriesIds)
            {
                target.Subs.Add(new CategorySubCategory() { CategoryId = target.CategoryId, SubCategoryId = subId });
            }
            if (category.Image is not null)
            {
                var imageId = target.ImageId;
                if (imageId is not null)
                {
                    DeleteImage(target.ImageId);
                }
                var imageresult = await UploadImage(category.Image);
                if (imageresult.Error is null)
                {
                    target.ImageUri = imageresult?.Uri.ToString();
                    target.ImageId = imageresult?.PublicId;
                }
            }
            _db.Update(target);
            await _db.SaveChangesAsync();
            return _mapper.Map<CategoryDTO>(target);
        }
        public async Task<Status> DeleteCategoryAsync(int categoryId)
        {
            var target = await _db.Categories.Include(c => c.Subs).ThenInclude(sc => sc.SubCategory).FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            if (target is null)
                return Status.NotFound;
            target.Subs.Clear();
            if (target.ImageId is not null)
                DeleteImage(target.ImageId);
            _db.Categories.Remove(target);
            await _db.SaveChangesAsync();
            return Status.Success;
        }
        public async Task<List<SubCategoryDTO>> GetAllSubCategoriesAsync()
        {
            return _mapper.Map<List<SubCategoryDTO>>(await _db.SubCategories.ToListAsync());
        }
        public async Task<SubCategoryDTO> GetSubCategoryById(int id)
        {
            return _mapper.Map<SubCategoryDTO>(await _db.SubCategories.Include(s => s.Categories).FirstOrDefaultAsync(s => s.SubId == id));
        }
        public async Task<SubCategoryDTO> AddSubCategoryAsync(SubCategoryAddDTO category)
        {
            var Subcategory = _mapper.Map<SubCategory>(category);
            var result = await _db.SubCategories.AddAsync(Subcategory);
            await _db.SaveChangesAsync();
            return _mapper.Map<SubCategoryDTO>(result.Entity);
        }
        public async Task<SubCategoryDTO> UpdateSubCategoryAsync(int subId, SubCategoryAddDTO subcat)
        {
            var target = await _db.SubCategories.FirstOrDefaultAsync(s => s.SubId == subId);
            if (target is null)
                return null;
            target.Name = subcat.Name;
            _db.Update(target);
            await _db.SaveChangesAsync();
            return _mapper.Map<SubCategoryDTO>(target);
        }
        public async Task<Status> DeleteSubCategoryAsync(int id)
        {
            var target = await _db.SubCategories.Include(s => s.Categories).FirstOrDefaultAsync(s => s.SubId == id);
            if (target is null)
                return Status.NotFound;
            //if(target.Categories.Any() || target.ProductSubCategories.Any())
            //    return Status.Failed;
            _db.SubCategories.Remove(target);
            await _db.SaveChangesAsync();
            return Status.Success;
        }
        public async Task<IEnumerable<SubCategoryDTO>> GetAllSubCategoriesForCategoryAsync(int categoryId)
        {
            return _mapper.Map<List<SubCategoryDTO>>(await _db.SubCategories.Where(s => s.Categories.Select(c => c.CategoryId).Contains(categoryId)).ToListAsync());
        }
        //public async Task<IEnumerable<ProductDisplayDTO>> GetAllProductsForSubCategoryAsync(int subId, string value)
        //{
        //    return _mapper.Map<List<ProductDisplayDTO>>(await _db.Products.Include(p => p.ca)
        //        .Include(p => p.Category)
        //        .Where(p => p.ProductSubCategories.Where(ps => ps.SubId == subId)
        //        .Any(ps => ps.SubCategoryValue == value)).ToListAsync());
        //    return null;
        //}
        public async Task<Status> AddProductPhotosAsync(ProductPictureDTO input)
        {
            List<Task<ImageUploadResult>> operations = new();
            var product = await _db.Products.FindAsync(input.ProductId);
            if (product == null) return Status.Failed;
            foreach (var picture in input.Pictures)
            {
                operations.Add(UploadImage(picture));
            }
            //Task.WaitAll([..operations]);
            var operationsResult = await Task.WhenAll(operations);
            foreach (var operationResult in operationsResult)
            {
                if (operationResult.Error is null)
                {
                    var pictureObject = new ProductImage()
                    {
                        ProductId = input.ProductId,
                        ImageUri = operationResult.Url.ToString(),
                        ImageId = operationResult.PublicId
                    };
                    _db.ProductImages.Add(pictureObject);
                }
            }
            await _db.SaveChangesAsync();
            return Status.Success;
        }
        private async Task<ImageUploadResult> UploadImage(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, file.OpenReadStream()),
                PublicId = $"images/uploads/{fileName}"
            };
            return await _cloudinary.UploadAsync(uploadParams);
        }
        public async Task<List<string>> GetProductPicturesAsync(int ProductId)
        {
            var product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.ProductId == ProductId);
            if (product == null) return null;
            return product.ProductImages.Select(p => p.ImageUri).ToList();
        }
        public async Task<Status> RemoveProductPictureAsync(int productId, string picture)
        {
            Product product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
                return Status.NotFound;
            ProductImage image = product.ProductImages.Where(image => image.ImageUri == picture).FirstOrDefault();
            if (image == null)
                return Status.NotFound;
            product.ProductImages.Remove(image);
            await DeleteImage(image.ImageId);
            await _db.SaveChangesAsync();
            return Status.Success;
        }
        public async Task DeleteImage(string ImageId)
        {
            await _cloudinary.DestroyAsync(new DeletionParams(ImageId));
        }
        public PagedResult<ProductDisplayDTO> RenderPagination(int page, int pageSize, List<Product> inputProducts)
        {
            PagedResult<ProductDisplayDTO> result = new PagedResult<ProductDisplayDTO>();
            int totalCount = inputProducts.Count;
            result.TotalItems = totalCount;
            result.TotalPages = totalCount / pageSize;
            if (totalCount % pageSize > 0)
                result.TotalPages++;
            result.PageSize = pageSize;
            result.PageNumber = page;
            result.HasPrevious = page != 1;
            result.HasNext = page != result.TotalPages;
            var products = inputProducts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            result.Items = _mapper.Map<List<ProductDisplayDTO>>(products);
            return result;
        }
        public async Task<PagedResult<ProductDisplayDTO>> RenderPaginationForAllProductsAsync(int page, int pageSize)
        {
            return RenderPagination(page, pageSize, await _db.Products.Include(p => p.Category).Include(p => p.ProductImages).ToListAsync());
        }
        public async Task<PagedResult<ProductDisplayDTO>> RenderSortedPaginationSortedAsync(int page, int pageSize, string sortOrder)
        {
            List<Product> productInput;
            if (sortOrder.Equals("name_asc"))
                productInput = _db.Products.OrderBy(p => p.Name).ToList();
            else if (sortOrder.Equals("name_des"))
                productInput = _db.Products.OrderByDescending(p => p.Name).ToList();
            else if (sortOrder.Equals("price_asc"))
                productInput = _db.Products.OrderBy(p => p.OriginalPrice).ToList();
            else if (sortOrder.Equals("price_des"))
                productInput = _db.Products.OrderByDescending(p => p.OriginalPrice).ToList();
            else if (sortOrder.Equals("amount_asc"))
                productInput = _db.Products.OrderBy(p => p.Amount).ToList();
            else if (sortOrder.Equals("amount_des"))
                productInput = _db.Products.OrderByDescending(p => p.Amount).ToList();
            else if (sortOrder.Equals("discount_asc"))
                productInput = _db.Products.OrderBy(p => p.Discount).ToList();
            else if (sortOrder.Equals("discount_des"))
                productInput = _db.Products.OrderByDescending(p => p.Discount).ToList();
            else
                productInput = await _db.Products.Include(p => p.Category).Include(p => p.ProductImages).OrderBy(p => p.Name).ToListAsync();
            return RenderPagination(page, pageSize, productInput);
        }
        public async Task<List<Product>> GetAllFilteredProductsFromSearchAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds)
        {
            var query = _db.Products.Include(p => p.Category).AsQueryable();
            if (!Name.IsNullOrEmpty())
                query = query.Where(p => p.Name.Contains(Name));
            if (MinOriginalPrice.HasValue)
                query = query.Where(p => p.OriginalPrice >= MinOriginalPrice);
            if (MaxOriginalPrice.HasValue)
                query = query.Where(p => p.OriginalPrice <= MinOriginalPrice);
            if (MinAmount.HasValue)
                query = query.Where(p => p.Amount >= MinAmount);
            if (MaxAmount.HasValue)
                query = query.Where(p => p.Amount <= MinAmount);
            if (CategoriesIds is not null)
                query = query.Where(p => CategoriesIds.Contains(p.CategoryId));
            return await query.ToListAsync();
        }
        public async Task<List<ProductDisplayDTO>> GetAllProductsSearchAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds)
        {
            var products = await GetAllFilteredProductsFromSearchAsync(Name, MinOriginalPrice, MaxOriginalPrice, MinAmount, MaxAmount, CategoriesIds);
            return _mapper.Map<List<ProductDisplayDTO>>(products);
        }
        public async Task<PagedResult<ProductDisplayDTO>> GetAllProductsSearchPaginatedAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, int page, int pageSize)
        {
            var products = await GetAllFilteredProductsFromSearchAsync(Name, MinOriginalPrice, MaxOriginalPrice, MinAmount, MaxAmount, CategoriesIds);
            var output = RenderPagination(page, pageSize, products);
            return output;
        }
        public async Task<bool> IsAllProductsExistsAsync(HashSet<int> productsIds)
        {
            var onBoth = productsIds.Intersect(_db.Products.Select(p => p.ProductId));
            if (onBoth.Count() == productsIds.Count)
                return true;
            return false;
        }
        public async Task<ProductCategorySubCategoryValuesDTO> GetProductCategorySubCategoryValuesAsync(int productId, int CategoryId, int SubCategoryId)
        {
            var categorySubCategory = await _db.CategorySubCategory.FirstOrDefaultAsync(cs => cs.CategoryId == CategoryId && cs.SubCategoryId == SubCategoryId);
            if (categorySubCategory is null)
                return null;
            var result = await _db.CategorySubCategoryValues
                .Include(c => c.CategorySubCategory).ThenInclude(cs => cs.Category)
                .Include(c => c.Product)
                .Include(c => c.CategorySubCategory)
                .ThenInclude(cs => cs.SubCategory)
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.CategorySubCategoryId == categorySubCategory.CategorySubCategoryId);
            if (result is null)
                return null;
            return _mapper.Map<ProductCategorySubCategoryValuesDTO>(result);
        }
        public async Task<List<ProductCategorySubCategoryValuesDTO>> GetAllProductCategorySubCategoryValuesAsync(int productId)
        {
            var result = await _db.CategorySubCategoryValues
                .Include(c => c.CategorySubCategory).ThenInclude(cs => cs.Category)
                .Include(c => c.Product)
                .Include(c => c.CategorySubCategory)
                .ThenInclude(cs => cs.SubCategory)
                .Where(c => c.ProductId == productId).ToListAsync();
            return _mapper.Map<List<ProductCategorySubCategoryValuesDTO>>(result);
        }
        public async Task<ProductCategorySubCategoryValuesDTO> AssignValueForProductCategorySubCategory(ProductCategorySubCategoyValueAddDTO input)
        {
            CategorySubCategoryValues newOne = new();
            if (input.Image is not null)
            {
                var imageoutput = await UploadImage(input.Image);
                if (imageoutput.Error is null)
                {
                    newOne.ImageUri = imageoutput.Uri.ToString();
                    newOne.ImageId = imageoutput.PublicId;
                }
            }
            newOne.ProductId = input.ProductId;
            var CategorySubCategoryId = await _db.CategorySubCategory.FirstOrDefaultAsync(cs => cs.CategoryId == input.CategoryId && cs.SubCategoryId == input.SubCategoryId);
            if (CategorySubCategoryId is null)
                return null;
            newOne.CategorySubCategoryId = CategorySubCategoryId.CategorySubCategoryId;
            newOne.Value = input.Value;
            var result = await _db.CategorySubCategoryValues.AddAsync(newOne);
            await _db.SaveChangesAsync();
            await _db.Entry(result.Entity).Reference(c => c.CategorySubCategory).LoadAsync();
            await _db.Entry(result.Entity).Reference(c => c.Product).LoadAsync();
            await _db.Entry(result.Entity).Reference(c => c.CategorySubCategory).TargetEntry.Reference(c => c.Category).LoadAsync();
            await _db.Entry(result.Entity).Reference(c => c.CategorySubCategory).TargetEntry.Reference(c => c.SubCategory).LoadAsync();

            return _mapper.Map<ProductCategorySubCategoryValuesDTO>(result.Entity);
        }
        public async Task<int> DeleteProductCategorySubCategoryValue(int productId, int categoryId, int subCategoryId, string value)
        {
            var CategorySubCategory = await _db.CategorySubCategory
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.SubCategoryId == subCategoryId);
            if (CategorySubCategory is null)
                return -1;
            int CategorySubCategoryId = CategorySubCategory.CategorySubCategoryId;
            var target = await _db.CategorySubCategoryValues
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.CategorySubCategoryId == CategorySubCategoryId && c.Value.ToLower() == value.ToLower());
            if (target is null)
                return -1;
            _db.CategorySubCategoryValues.Remove(target);
            await _db.SaveChangesAsync();
            return 1;
        }
        public async Task<int> DeleteProductCategorySubCategoryValueAll(int productId, int categoryId, int subCategoryId)
        {
            var CategorySubCategory = await _db.CategorySubCategory
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.SubCategoryId == subCategoryId);
            if (CategorySubCategory is null)
                return -1;
            int CategorySubCategoryId = CategorySubCategory.CategorySubCategoryId;
            var target = _db.CategorySubCategoryValues
                .Where(c => c.ProductId == productId && c.CategorySubCategoryId == CategorySubCategoryId);
            if (target is null)
                return -1;
            _db.CategorySubCategoryValues.RemoveRange(target);
            await _db.SaveChangesAsync();
            return 1;
        }
        public async Task<ProductCategorySubCategoryValuesDTO> UpdateProductCategorySubCategoryValue(ProductCategorySubCategoyValueAddDTO addDTO, string newValue)
        {
            var CategorySubCategory = await _db.CategorySubCategory
                .FirstOrDefaultAsync(c => c.CategoryId == addDTO.ProductId && c.SubCategoryId == addDTO.SubCategoryId);
            if (CategorySubCategory is null)
                return null;
            int CategorySubCategoryId = CategorySubCategory.CategorySubCategoryId;
            var target = await _db.CategorySubCategoryValues
                .FirstOrDefaultAsync(c => c.ProductId == addDTO.ProductId && c.CategorySubCategoryId == CategorySubCategoryId && c.Value.ToLower() == addDTO.Value.ToLower());
            if (target is null)
                return null;
            target.Value = newValue;
            if(addDTO.Image is not null)
            {
                var imageoutput = await UploadImage(addDTO.Image);
                if (imageoutput.Error is null)
                {
                    target.ImageUri = imageoutput.Uri.ToString();
                    target.ImageId = imageoutput.PublicId;
                }
            }
            else
            {
                if(target.ImageId != null && target.ImageUri != null)
                {
                    DeleteImage(target.ImageId);
                    target.ImageUri = target.ImageId = null;
                }
            }
            _db.Update(target);
            await _db.SaveChangesAsync();
            await _db.Entry(target).Reference(t => t.Product).LoadAsync();
            await _db.Entry(target).Reference(t => t.CategorySubCategory).TargetEntry.Reference(te => te.Category).LoadAsync();
            await _db.Entry(target).Reference(t => t.CategorySubCategory).TargetEntry.Reference(te => te.SubCategory).LoadAsync();
            return _mapper.Map<ProductCategorySubCategoryValuesDTO>(target);
        }

    }
}
