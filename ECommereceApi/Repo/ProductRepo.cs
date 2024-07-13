using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Services.Interfaces;
using Serilog;
using ECommereceApi.DTOs.Order;

namespace ECommereceApi.Repo
{
    public class ProductRepo : IProductRepo
    {
        private readonly ECommerceContext _db;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IFileCloudService _fileCloudService;
        public ProductRepo(IWebHostEnvironment env, ECommerceContext context, IMapper mapper, IFileCloudService fileCloudService, ICategoryRepo categoryRepo)
        {
            _env = env;
            _db = context;
            _mapper = mapper;
            _fileCloudService = fileCloudService;
            _categoryRepo = categoryRepo;
        }


        public async Task<IEnumerable<ProductDisplayDTO>> GetAllProductDisplayDTOsAsync()
        {
            var AllIds = await GetAllProductsIdsAsync();
            return await GetProductDisplayDTOsByIdsAsync(AllIds);
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
        public async Task<ICollection<ProductDisplayDTO>> GetProductsDisplayDTOsFromCategorySubCategoryIdAndValueAsync(int categorySubCategoryId, string value)
        {
            return _mapper.Map<List<ProductDisplayDTO>>(await GetProductsFromCategorySubCategoryIdsValuesAsync(categorySubCategoryId, value));
        }


        public async Task UpdateProductAsync(ProductAddDTO product, int Id)
        {
            var target = await _db.Products.SingleOrDefaultAsync(p => p.ProductId == Id);
            _mapper.Map(product, target);
            _db.Products.Update(target);
            await MySaveChangesAsync();
        }
        public async Task<ProductDisplayDTO> AddProductAsync(ProductAddDTO product)
        {
            var result = await _db.Products.AddAsync(_mapper.Map<Product>(product));
            await MySaveChangesAsync();
            return await GetProductDisplayDTOByIdAsync(result.Entity.ProductId);
        }
        public async Task<ICollection<ProductDisplayDTO>> GetProductDisplayDTOsByIdsAsync(ICollection<int> ids)
        {
            var output = new List<ProductDisplayDTO>();
            foreach (var id in ids)
                output.Add(await GetProductDisplayDTOByIdAsync(id));
            return output;
        }
        public async Task DeleteProductAsync(int id)
        {
            var product = await _db.Products.Include(p => p.ProductOffers).FirstOrDefaultAsync(p => p.ProductId == id);
            _db.Products.Remove(product);
            await MySaveChangesAsync();
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
        public async Task<IEnumerable<ProductDisplayDTO>> GetAllCategoryProductsAsync(int categoryId)
        {
            return _mapper.Map<List<ProductDisplayDTO>>(await _db.Products.Where(p => p.CategoryId == categoryId).Include(p => p.Category).Include(p => p.ProductImages).ToListAsync());
        }
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
                    ImageId = operationResult,
                    ImageUri = _fileCloudService.GetImageUrl(operationResult)
                };
                _db.ProductImages.Add(pictureObject);
            }
            await MySaveChangesAsync();
            return Status.Success;
        }
        public async Task<List<string>> GetProductPicturesAsync(int ProductId)
        {
            var product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.ProductId == ProductId);
            return product.ProductImages.Select(p => p.ImageUri).ToList();
        }
        public async Task RemoveProductPictureAsync(int productId, string pictureId)
        {
            Product product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.ProductId == productId);
            ProductImage image = product.ProductImages.Where(image => image.ImageId == pictureId).FirstOrDefault();
            product.ProductImages.Remove(image);
            _fileCloudService.DeleteImageAsync(image.ImageId);
            await MySaveChangesAsync();
        }
        public async Task<PagedResult<ProductDisplayDTO>> RenderPaginationForAllProductsAsync(int page, int pageSize)
        {
            return RenderPagination(page, pageSize, await _db.Products.Include(p => p.Category).Include(p => p.ProductImages).ToListAsync());
        }
        public async Task<PagedResult<ProductDisplayDTO>> RenderSortedPaginationSortedAsync(int page, int pageSize, string sortOrder)
        {
            var query = GetBasicProductQuery();
            if (sortOrder.Equals("name_asc"))
                query = query.OrderBy(p => p.Name);
            else if (sortOrder.Equals("name_des"))
                query = query.OrderByDescending(p => p.Name);
            else if (sortOrder.Equals("price_asc"))
                query = query.OrderBy(p => p.OriginalPrice);
            else if (sortOrder.Equals("price_des"))
                query = query.OrderByDescending(p => p.OriginalPrice);
            else if (sortOrder.Equals("amount_asc"))
                query = query.OrderBy(p => p.Amount);
            else if (sortOrder.Equals("amount_des"))
                query = query.OrderByDescending(p => p.Amount);
            else if (sortOrder.Equals("discount_asc"))
                query = query.OrderBy(p => p.Discount);
            else if (sortOrder.Equals("discount_des"))
                query = query.OrderByDescending(p => p.Discount);
            return RenderPagination(page, pageSize, await query.ToListAsync());
        }
        public async Task<List<Product>> GetAllFilteredProductsFromSearchAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, int? OfferId)
        {
            var query = GetBasicProductQuery();
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
        public async Task<int> DeleteProductCategorySubCategoryValue(int productId, int categoryId, int subCategoryId, string value)
        {
            var categorySubCategoryValueId = await _categoryRepo.GetCategorySubCategoryValueIdAsync(categoryId, subCategoryId, value);
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
        public async Task<bool> IsProductExistsAsync(int id)
        {
            return await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id) is not null;
        }
        public async Task MySaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
        public async Task<ICollection<int>> GetAllProductsIdsAsync()
        {
            return await _db.Products.Select(p => p.ProductId).ToListAsync();
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
        public async Task<bool> IsProductImageExistsAsync(int productId, string imageId)
        {
            return await _db.Products.Include(p => p.ProductImages).AnyAsync(p => p.ProductId == productId && p.ProductImages.Any(pi => pi.ImageId == imageId));
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
        public IQueryable<Product>? GetBasicProductQuery()
        {
            return _db.Products
                .Include(p => p.Category)
                .Include(p => p.ProductOffers)
                .Include(p => p.ProductImages)
                .AsQueryable();
        }
        public async Task<bool> IsAllProductsExistsAsync(HashSet<int> productsIds)
        {
            var onBoth = productsIds.Intersect(_db.Products.Select(p => p.ProductId));
            if (onBoth.Count() == productsIds.Count)
                return true;
            return false;
        }
        public bool IsOrgignalPriceGreaterThanDiscount(double originalPrice, double? discount)
        {
            if (discount is null)
                return true;
            else
                return originalPrice > discount;
        }
    }
}
