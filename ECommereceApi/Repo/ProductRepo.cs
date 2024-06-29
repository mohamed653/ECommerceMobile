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
using ECommereceApi.Services.classes;
using ECommereceApi.Services.Interfaces;
using System.Collections.Generic;
using ECommereceApi.DTOs.Order;
using Serilog;

namespace ECommereceApi.Repo
{
    public class ProductRepo : IProductRepo
    {
        private readonly ECommerceContext _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IFileCloudService _fileCloudService;
        public ProductRepo(IWebHostEnvironment env, ECommerceContext context, IMapper mapper, IFileCloudService fileCloudService)
        {
            _env = env;
            _db = context;
            _mapper = mapper;
            _fileCloudService = fileCloudService;
        }
        public async Task<ProductDisplayDTO> AddProductAsync(ProductAddDTO product)
        {
            var result = await _db.Products.AddAsync(_mapper.Map<Product>(product));
            //if (product.SubCategoriesIds.Count != product.SubCategoriesValues.Count)
            //    return null;
            await MySaveChangesAsync();
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
            //await MySaveChangesAsync()
            return await GetProductDisplayDTOByIdAsync(result.Entity.ProductId);
        }
        public async Task<bool> IsProductExistsAsync(int id)
        {
            return await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id) is not null;
        }
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _db.Products
                .Include(p => p.ProductCarts)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductOffers)
                .Include(p => p.ProductOrders)
                .FirstOrDefaultAsync(p => p.ProductId == id);
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
            await MySaveChangesAsync();
            return Status.Success;
        }
        public async Task MySaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
        public async Task<IEnumerable<ProductDisplayDTO>> GetAllProductDisplayDTOsAsync()
        {
            var AllIds = await GetAllProductsIdsAsync();
            return await GetProductDisplayDTOsByIdsAsync(AllIds);
        }
        public async Task<ICollection<int>> GetAllProductsIdsAsync()
        {
            return await _db.Products.Select(p => p.ProductId).ToListAsync();
        }
        public async Task<ICollection<ProductDisplayDTO>> GetProductDisplayDTOsByIdsAsync(ICollection<int> ids)
        {
            var output = new List<ProductDisplayDTO>();
            foreach (var id in ids)
                output.Add(await GetProductDisplayDTOByIdAsync(id));
            return output;
        }
        public async Task<ProductDisplayDTO> GetProductDisplayDTOByIdAsync(int id)
        {
            var productSubCategoryValues = await GetProductSubCategoryValuesGroupedBySubCategoryByIdAsync(id);
            var output = await GetBasicProductDisplayDTOByIdAsync(id);
            MapSubCategoryValuesIntoProductDisplayDTOAsync(output, productSubCategoryValues);
            return output;
        }
        public async Task MapSubCategoryValuesIntoProductDisplayDTOAsync(ProductDisplayDTO productDisplayDTO, ICollection<IGrouping<SubCategory, ProductCategorySubCategoryValues>> subCategoryValues)
        {
            foreach (var group in subCategoryValues)
            {
                SubCategoryValuesDTO oneToAdd = new()
                {
                    Name = group.Key.Name,
                    SubCategoryId = group.Key.SubCategoryId,
                };
                foreach (var sub in group)
                {
                    SubCategoryValuesDetailsDTO details = new();
                    details.Value = sub.CategorySubCategoryValues.Value;
                    details.ImageId = sub.CategorySubCategoryValues.ImageId;
                    details.ImageUrl = _fileCloudService.GetImageUrl(sub.CategorySubCategoryValues.ImageId);
                    oneToAdd.Values.Add(details);
                }
                productDisplayDTO.CategoryValues.Add(oneToAdd);
            }
        }
        public async Task<ProductDisplayDTO> GetBasicProductDisplayDTOByIdAsync(int id)
        {
            return _mapper.Map<ProductDisplayDTO>(await _db.Products
                .Include(p => p.Category)
                .ThenInclude(c => c.CategorySubCategory)
                .ThenInclude(cs => cs.CategorySubCategoryValues)
                .Include(p => p.ProductImages)
                .SingleOrDefaultAsync(p => p.ProductId == id));
        }
        public async Task<ICollection<IGrouping<SubCategory, ProductCategorySubCategoryValues>>> GetProductSubCategoryValuesGroupedBySubCategoryByIdAsync(int id)
        {
            return await _db.ProductCategorySubCategoryValues
                .Include(pcsv => pcsv.CategorySubCategoryValues)
                .ThenInclude(csv => csv.CategorySubCategory)
                .ThenInclude(cs => cs.SubCategory)
                .Where(pcsv => pcsv.ProductId == id)
                .GroupBy(p => p.CategorySubCategoryValues.CategorySubCategory.SubCategory)
                .ToListAsync();
        }
        public async Task<Status> UpdateProductAsync(ProductAddDTO product, int Id)
        {
            var target = await _db.Products.SingleOrDefaultAsync(p => p.ProductId == Id);
            if (target == null)
                return Status.NotFound;
            _mapper.Map(product, target);
            _db.Products.Update(target);
            await MySaveChangesAsync();
            return Status.Success;
        }

        //***Hamed***
        public async Task SubtractProductAmountFromStock(List<ProductOrderStockDTO> productOrderStockDTOs)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var productOrderStockDTO in productOrderStockDTOs)
                    {
                        var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == productOrderStockDTO.ProductId);
                        if (product == null)
                            continue;

                        if (product.Amount < productOrderStockDTO.Amount)
                        {
                            throw new Exception($"The amount of the product {product.ProductId} is less than the amount of the order");
                        }

                        product.Amount -= productOrderStockDTO.Amount;
                        _db.Products.Update(product);
                    }

                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error("Error in SubtractProductAmountFromStock: {0}", ex.Message);
                    throw;
                }
            }
        }

        public async Task<IEnumerable<ProductDisplayDTO>> GetAllCategoryProductsAsync(int categoryId)
        {
            return _mapper.Map<List<ProductDisplayDTO>>(await _db.Products.Where(p => p.CategoryId == categoryId).Include(p => p.Category).Include(p => p.ProductImages).ToListAsync());
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
            }
            foreach (var subId in category.SubCategoriesIds)
            {
                result.Entity.CategorySubCategory.Add(new CategorySubCategory() { CategoryId = result.Entity.CategoryId, SubCategoryId = subId });
            }
            await MySaveChangesAsync();
            return _mapper.Map<CategoryDTO>(result.Entity);
        }
        public async Task<CategoryDTO> UpdateCategoryAsync(int id, CategoryAddDTO category)
        {
            var target = await _db.Categories.Include(c => c.CategorySubCategory).ThenInclude(sc => sc.SubCategory).FirstOrDefaultAsync(c => c.CategoryId == id);
            if (target is null)
                return null;
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
        public async Task<SubCategoryDTO> AddSubCategoryAsync(SubCategoryAddDTO category)
        {
            var Subcategory = _mapper.Map<SubCategory>(category);
            var result = await _db.SubCategories.AddAsync(Subcategory);
            await MySaveChangesAsync();
            return _mapper.Map<SubCategoryDTO>(result.Entity);
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
            }
            var output = _mapper.Map<CategorySubCategoryValueDTO>(input);
            output.ImageId = result.Entity?.ImageId;
            if (result.Entity.ImageId is not null)
                output.ImageUrl = _fileCloudService.GetImageUrl(result.Entity.ImageId);
            await MySaveChangesAsync();
            output.Id = result.Entity.Id;
            return output;
        }
        public async Task<SubCategoriesValuesForCategoryDTO> GetCategoryDetailsAsync(int categoryId)
        {
            var category = await GetCategoryWithSubCategoryWithValuesAsync(categoryId);
            return await MapSubCategoriesValuesForCategory(category);
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
        public async Task<ICollection<SubCategoriesValuesForCategoryDTO>> GetAllCategoriesDetailsAsync()
        {
            var CategoriesIds = await GetAllCategoriesIdsAsync();
            return await GetAllCategoriesDetailsFromIdsAsync(CategoriesIds);
        }
        public async Task<ICollection<int>> GetAllCategoriesIdsAsync()
        {
            return await _db.Categories.Select(c => c.CategoryId).ToListAsync();
        }
        public async Task<CategoriesValuesForSubCategoryDTO> GetSubCategoryDetails(int subCategoryId)
        {
            var subCategory = await GetSubCategoryWithCategoryValuesAsync(subCategoryId);
            return await MapCategoriesValuesForSubCategory(subCategory);
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
        public async Task<ICollection<SubCategory>> GetSubCategoriesForCategoryAsync(Category category)
        {
            return category.CategorySubCategory.Select(c => c.SubCategory).ToList();
        }
        public async Task<ICollection<Category>> GetCategoriesForSubCategoryAsync(SubCategory subCategory)
        {
            return subCategory.CategorySubCategories.Select(c => c.Category).ToList();
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
        public async Task<int> AssignSubCategoryToCategoryAsync(int categoryId, int subCategoryId)
        {
            var result = await _db.CategorySubCategory.AddAsync(new CategorySubCategory() { CategoryId = categoryId, SubCategoryId = subCategoryId });
            await MySaveChangesAsync();
            return result.Entity.CategorySubCategoryId;
        }
        public Task<int> GetCategorySubCategoryIdFromSeparateIds(int categoryId, int subCategoryId)
        {
            return _db.CategorySubCategory.Where(cs => cs.CategoryId == categoryId && cs.SubCategoryId == subCategoryId).Select(cs => cs.CategorySubCategoryId).FirstOrDefaultAsync();
        }
        public async Task<bool> IsCategoryExistsAsync(int id)
        {
            return await _db.Categories.AnyAsync(c => c.CategoryId == id);
        }
        public async Task<bool> IsSubCategoryExistsAsync(int id)
        {
            return await _db.SubCategories.AnyAsync(s => s.SubCategoryId == id);
        }
        public async Task<bool> IsCategorySubCategoryExistsAsync(int categoryId, int subCategoryId)
        {
            return await _db.CategorySubCategory.AnyAsync(cs => cs.CategoryId == categoryId && cs.SubCategoryId == subCategoryId);
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
        public async Task<IEnumerable<SubCategoryDTO>> GetAllSubCategoriesForCategoryAsync(int categoryId)
        {
            return _mapper.Map<List<SubCategoryDTO>>(await _db.SubCategories.Where(s => s.CategorySubCategories.Select(c => c.CategoryId).Contains(categoryId)).ToListAsync());
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
            List<Task<string>> operations = new();
            var product = await _db.Products.FindAsync(input.ProductId);
            if (product == null) return Status.Failed;
            foreach (var picture in input.Pictures)
            {
                //operations.Add(UploadImage(picture));
                operations.Add(_fileCloudService.UploadImagesAsync(picture));
            }
            //Task.WaitAll([..operations]);
            var operationsResult = await Task.WhenAll(operations);
            foreach (var operationResult in operationsResult)
            {
                var pictureObject = new ProductImage()
                {
                    ProductId = input.ProductId,
                    ImageId = operationResult
                };
                _db.ProductImages.Add(pictureObject);

            }
            await MySaveChangesAsync();
            return Status.Success;
        }
        //private async Task<ImageUploadResult> UploadImage(IFormFile file)
        //{
        //    var fileName = Guid.NewGuid().ToString();
        //    var uploadParams = new ImageUploadParams()
        //    {
        //        File = new FileDescription(fileName, file.OpenReadStream()),
        //        PublicId = $"images/uploads/{fileName}"
        //    };
        //    return await _cloudinary.UploadAsync(uploadParams);
        //}
        public async Task<List<string>> GetProductPicturesAsync(int ProductId)
        {
            var product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.ProductId == ProductId);
            if (product == null) return null;
            return product.ProductImages.Select(p => _fileCloudService.GetImageUrl(p.ImageId)).ToList();
        }
        public async Task<Status> RemoveProductPictureAsync(int productId, string picture)
        {
            Product product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
                return Status.NotFound;
            ProductImage image = product.ProductImages.Where(image => _fileCloudService.GetImageUrl(image.ImageId) == picture).FirstOrDefault();
            if (image == null)
                return Status.NotFound;
            product.ProductImages.Remove(image);
            //DeleteImage(image.ImageId);
            _fileCloudService.DeleteImageAsync(image.ImageId);
            await MySaveChangesAsync();
            return Status.Success;
        }
        //public async Task DeleteImage(string ImageId)
        //{
        //    await _cloudinary.DestroyAsync(new DeletionParams(ImageId));
        //}
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
        public async Task<List<Product>> GetAllFilteredProductsFromSearchAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, int? OfferId)
        {
            var query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.ProductOffers)
                .AsQueryable();
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
            if (OfferId is not null)
                query = query.Where(p => p.ProductOffers.Any(po => po.OfferId != OfferId));
            return await query.ToListAsync();
        }
        public async Task<List<ProductDisplayDTO>> GetAllProductsSearchAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, int? offerId)
        {
            var products = await GetAllFilteredProductsFromSearchAsync(Name, MinOriginalPrice, MaxOriginalPrice, MinAmount, MaxAmount, CategoriesIds, offerId);
            return _mapper.Map<List<ProductDisplayDTO>>(products);
        }
        public async Task<PagedResult<ProductDisplayDTO>> GetAllProductsSearchPaginatedAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, int page, int pageSize, int? offerId)
        {
            var products = await GetAllFilteredProductsFromSearchAsync(Name, MinOriginalPrice, MaxOriginalPrice, MinAmount, MaxAmount, CategoriesIds, offerId);
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
            var result = await _db.ProductCategorySubCategoryValues
                .Include(c => c.CategorySubCategoryValues).ThenInclude(cs => cs.CategorySubCategory).ThenInclude(c => c.Category)
                .Include(c => c.Product)
                .Include(c => c.CategorySubCategoryValues).ThenInclude(c => c.CategorySubCategory)
                .ThenInclude(cs => cs.SubCategory)
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.CategorySubCategoryValues.CategorySubCategoryId == categorySubCategory.CategorySubCategoryId);
            if (result is null)
                return null;
            return _mapper.Map<ProductCategorySubCategoryValuesDTO>(result);
        }
        public async Task<List<ProductCategorySubCategoryValuesDTO>> GetAllProductCategorySubCategoryValuesAsync(int productId)
        {
            var result = await _db.ProductCategorySubCategoryValues
                .Include(c => c.CategorySubCategoryValues)
                .ThenInclude(c => c.CategorySubCategory)
                .ThenInclude(cs => cs.Category)
                .Include(cs => cs.CategorySubCategoryValues)
                .ThenInclude(cs => cs.CategorySubCategory)
                .ThenInclude(cs => cs.SubCategory)
                .Include(c => c.Product)
                .Where(c => c.ProductId == productId).ToListAsync();
            return _mapper.Map<List<ProductCategorySubCategoryValuesDTO>>(result);
        }
        public async Task<ProductCategorySubCategoryValuesDTO> AssignValueForProductCategorySubCategory(ProductCategorySubCategoyValueAddDTO input)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == input.ProductId);
            if (product is null)
                return null;
            var categorySubCategory = await _db.CategorySubCategory
                .Include(cs => cs.Category)
                .Include(cs => cs.SubCategory)
                .FirstOrDefaultAsync(cs => cs.Category.CategoryId == input.CategoryId && cs.SubCategory.SubCategoryId == input.SubCategoryId);
            if (categorySubCategory is null)
                return null;
            var CategorySubCategoryValue = await _db.CategorySubCategoryValues.FirstOrDefaultAsync(cs => cs.CategorySubCategoryId == categorySubCategory.CategorySubCategoryId && cs.Value == input.Value);
            if (CategorySubCategoryValue is null)
                return null;
            var result = _db.ProductCategorySubCategoryValues.Add(new ProductCategorySubCategoryValues()
            {
                CategorySubCategoryValuesId = CategorySubCategoryValue.Id,
                ProductId = input.ProductId
            });
            await MySaveChangesAsync();
            await _db.Entry(result.Entity).Reference(c => c.CategorySubCategoryValues).TargetEntry.Reference(c => c.CategorySubCategory).LoadAsync();
            await _db.Entry(result.Entity).Reference(c => c.Product).LoadAsync();
            await _db.Entry(result.Entity).Reference(c => c.CategorySubCategoryValues).TargetEntry.Reference(c => c.CategorySubCategory).TargetEntry.Reference(c => c.Category).LoadAsync();
            await _db.Entry(result.Entity).Reference(c => c.CategorySubCategoryValues).TargetEntry.Reference(c => c.CategorySubCategory).TargetEntry.Reference(c => c.SubCategory).LoadAsync();

            return _mapper.Map<ProductCategorySubCategoryValuesDTO>(result.Entity);
        }
        public async Task<int> GetCategorySubCategoryValueIdAsync(int categoryId, int subCategoryId, string value)
        {
            var output = await _db.CategorySubCategoryValues
                .FirstOrDefaultAsync(c => c.CategorySubCategory.CategoryId == categoryId && c.CategorySubCategory.SubCategoryId == subCategoryId && c.Value.ToLower() == value.ToLower());
            if (output is null)
                return -1;
            return output.Id;
        }
        public async Task<int> DeleteProductCategorySubCategoryValue(int productId, int categoryId, int subCategoryId, string value)
        {
            var categorySubCategoryValueId = await GetCategorySubCategoryValueIdAsync(categoryId, subCategoryId, value);
            if (categorySubCategoryValueId == -1)
                return -1;
            var recordToDelete = await _db.ProductCategorySubCategoryValues.FirstOrDefaultAsync(pc => pc.CategorySubCategoryValuesId == categorySubCategoryValueId && pc.ProductId == productId);
            if (recordToDelete is null)
                return -1;
            _db.ProductCategorySubCategoryValues.Remove(recordToDelete);
            await MySaveChangesAsync();
            return 1;
        }
        public async Task<int> DeleteProductCategorySubCategoryValueAll(int productId, int categoryId, int subCategoryId)
        {
            var CategorySubCategory = await _db.CategorySubCategory
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.SubCategoryId == subCategoryId);
            if (CategorySubCategory is null)
                return -1;
            int CategorySubCategoryId = CategorySubCategory.CategorySubCategoryId;
            var target = _db.ProductCategorySubCategoryValues
                .Where(c => c.ProductId == productId && c.CategorySubCategoryValues.CategorySubCategoryId == CategorySubCategoryId);
            if (target is null)
                return -1;
            _db.ProductCategorySubCategoryValues.RemoveRange(target);
            await MySaveChangesAsync();
            return 1;
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
                }
                else
                {
                    var output = await _fileCloudService.UploadImagesAsync(addDTO.Image);
                    target.ImageId = output;
                }
            }
            else
            {
                if (target.ImageId != null)
                {
                    var result = await _fileCloudService.DeleteImageAsync(target.ImageId);
                    if (result)
                        target.ImageId = null;
                }
            }
            _db.Update(target);
            await MySaveChangesAsync();
            await _db.Entry(target).Reference(t => t.CategorySubCategory).TargetEntry.Reference(te => te.Category).LoadAsync();
            await _db.Entry(target).Reference(t => t.CategorySubCategory).TargetEntry.Reference(te => te.SubCategory).LoadAsync();
            return _mapper.Map<ProductCategorySubCategoryValuesDTO>(target);
        }
        public async Task<ICollection<ProductDisplayDTO>> GetProductsDisplayDTOsFromCategorySubCategoryIdAndValueAsync(int categorySubCategoryId, string value)
        {
            return _mapper.Map<List<ProductDisplayDTO>>(await GetProductsFromCategorySubCategoryIdsValuesAsync(categorySubCategoryId, value));
        }
        public async Task<ICollection<Product>> GetProductsFromCategorySubCategoryIdsValuesAsync(int categorySubCategoryId, string value)
        {
            var allProcutsFromCategorySubCategory = await GetProductsFromCategorySubCategoryIdAsync(categorySubCategoryId);
            return await _db.ProductCategorySubCategoryValues
                .Include(csv => csv.Product)
                .Include(csv => csv.CategorySubCategoryValues)
                .Where(csv => allProcutsFromCategorySubCategory.Contains(csv.Product) && csv.CategorySubCategoryValues.Value == value)
                .Select(csv => csv.Product).ToListAsync();
        }
        public async Task<ICollection<Product>> GetProductsFromCategorySubCategoryIdAsync(int categorySubCategoryId)
        {
            return await _db.Products
                .Include(p => p.Category)
                .ThenInclude(c => c.CategorySubCategory)
                .ThenInclude(cs => cs.CategorySubCategoryValues)
                .Where(p => p.Category.CategorySubCategory.Any(cs => cs.CategorySubCategoryId == categorySubCategoryId)).ToListAsync();
        }
    }
}
