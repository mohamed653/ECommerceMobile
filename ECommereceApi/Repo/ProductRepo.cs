using ECommereceApi.Models;
using ECommereceApi.IRepo;
using ECommereceApi.Data;
using Microsoft.EntityFrameworkCore;
using ECommereceApi.DTOs;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Azure;

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
            await _db.SaveChangesAsync();
            return await GetProductByIdAsync(result.Entity.ProductId);
        }

        public async Task<Status> DeleteProductAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return Status.Failed;
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return Status.Success;
        }

        public async Task<IEnumerable<ProductDisplayDTO>> GetAllProductsAsync()
        {
            return _mapper.Map<List<ProductDisplayDTO>>(await _db.Products.Include(p => p.Category).ToListAsync()); ;
        }

        public async Task<ProductDisplayDTO> GetProductByIdAsync(int id)
        {
            return _mapper.Map<ProductDisplayDTO>(await _db.Products.Include(p => p.Category).SingleOrDefaultAsync(p => p.ProductId == id));
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
            return _mapper.Map<List<ProductDisplayDTO>>(await _db.Products.Where(p => p.CategoryId == categoryId).Include(p => p.Category).ToListAsync());
        }
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            return _mapper.Map<List<CategoryDTO>>(await _db.Categories.ToListAsync());
        }
        public async Task<IEnumerable<CategoryDTO>> GetAllSubCategoriesForCategoryAsync(int categoryId)
        {
            return _mapper.Map<List<CategoryDTO>>(await _db.SubCategories.Where(s => s.Categories.Select(c => c.CategoryId).Contains(categoryId)).ToListAsync());
        }
        public async Task<IEnumerable<ProductDisplayDTO>> GetAllProductsForSubCategoryAsync(int subId, string value)
        {
            return _mapper.Map<List<ProductDisplayDTO>>(await _db.Products.Include(p => p.ProductSubCategories)
                .Include(p => p.Category)
                .Where(p => p.ProductSubCategories.Where(ps => ps.SubId == subId)
                .Any(ps => ps.SubCategoryValue == value)).ToListAsync());
        }
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
            DeletionParams del = new DeletionParams(image.ImageId);
            await _cloudinary.DestroyAsync(del);
            await _db.SaveChangesAsync();
            return Status.Success;
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
            return RenderPagination(page, pageSize, await _db.Products.Include(p => p.Category).ToListAsync());
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
                productInput = await _db.Products.Include(p => p.Category).OrderBy(p => p.Name).ToListAsync();
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

    }
}
