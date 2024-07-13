using ECommereceApi.DTOs.Product;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using ECommereceApi.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo _productRepo;
        private readonly ICartRepo _cartRepo;
        private readonly IOfferRepo _offerRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly ICategoryRepo _categoryRepo;
        public ProductController(IProductRepo productRepo, ICartRepo cartRepo, IOfferRepo offerRepo, IOrderRepo orderRepo, ICategoryRepo categoryRepo)
        {
            _productRepo = productRepo;
            _cartRepo = cartRepo;
            _offerRepo = offerRepo;
            _orderRepo = orderRepo;
            _categoryRepo = categoryRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            return Ok(await _productRepo.GetAllProductDisplayDTOsAsync());
        }
        [HttpPost]
        public async Task<IActionResult> AddProductAsync([Required] ProductAddDTO product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_productRepo.IsOrgignalPriceGreaterThanDiscount(product.OriginalPrice, product.Discount))
                return BadRequest("Price can't Be Equal or less than Discount!");
            if (!await _categoryRepo.IsCategoryExistsAsync(product.CategoryId))
                return NotFound("Category Not Found");
            var result = await _productRepo.AddProductAsync(product);
            if (result is null)
                return BadRequest();
            return Created("", result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            if (!await _productRepo.IsProductExistsAsync(id)) return NotFound("No Product with this Id");
            if (await _cartRepo.IsProductExistsInAnyCartAsync(id))
                return BadRequest("Product Is In Cart");
            if (await _offerRepo.IsProductInActiveOrComingOfferAsync(id))
                return BadRequest("Product Is included In Active Or Coming Offer");
            if (await _orderRepo.IsProductInActiveOrderAsync(id))
                return BadRequest("Product Is In Active Order");
            await _productRepo.DeleteProductAsync(id);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync([Required] ProductAddDTO product, int Id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_productRepo.IsOrgignalPriceGreaterThanDiscount(product.OriginalPrice, product.Discount))
                return BadRequest("Price can't Be Equal or less than Discount!");
            if (!await _categoryRepo.IsCategoryExistsAsync(product.CategoryId))
                return NotFound("Category Not Found");
            await _productRepo.UpdateProductAsync(product, Id);
            return Ok();
        }
        [HttpGet]
        [Route("/api/product/{id:int}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            if (!await _productRepo.IsProductExistsAsync(id))
                return NotFound("Product Doesn't Exist");
            var result = await _productRepo.GetProductDisplayDTOByIdAsync(id);
            return Ok(result);
        }
        [HttpGet]
        [Route("/api/category/{categoryId:int}")]
        public async Task<IActionResult> GetAllProductsByCategoryAsync(int categoryId)
        {
            if (!await _categoryRepo.IsCategoryExistsAsync(categoryId))
                return NotFound("Category Not Found!");
            var result = await _productRepo.GetAllCategoryProductsAsync(categoryId);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/Product/CategorySubCategoryValue")]
        public async Task<IActionResult> GetAllProductsForCategorySubCategoryValues([Required] int categoryId, [Required] int subCategoryId, [Required] string value)
        {
            if (value.IsNullOrEmpty()) return BadRequest("Invalid Value");
            if (!await _categoryRepo.IsCategoryExistsAsync(categoryId)) return NotFound("Category Not Found!");
            if (!await _categoryRepo.IsSubCategoryExistsAsync(subCategoryId)) return NotFound("Sub Category Not Found");
            int? categorySubCategoryId = await _categoryRepo.GetCategorySubCategoryIdFromSeparateIds(categoryId, subCategoryId);
            if (categorySubCategoryId is null) return NotFound("Category And Sub Category Not Related");
            return Ok(await _productRepo.GetProductsDisplayDTOsFromCategorySubCategoryIdAndValueAsync(categorySubCategoryId.Value, value));
        }

        [HttpDelete]
        [Route("/api/ProductCategorySubCategoryValues")]
        public async Task<IActionResult> DeleteProductCategorySubCategoryValueAsync(int productId, int categoryId, int subCategoryId, string value)
        {
            if (!await _productRepo.IsProductExistsAsync(productId))
                return NotFound("Product Not Found!");
            if (!await _categoryRepo.IsCategoryExistsAsync(categoryId))
                return NotFound("Category Not Found!");
            if (!await _categoryRepo.IsSubCategoryExistsAsync(subCategoryId))
                return NotFound("Sub Category Not Found!");
            int? categorySubCategoryId = await _categoryRepo.GetCategorySubCategoryIdFromSeparateIds(categoryId, subCategoryId);
            if (categorySubCategoryId is null) return NotFound("Category And Sub Category Not Related");
            var result = await _productRepo.DeleteProductCategorySubCategoryValue(productId, categoryId, subCategoryId, value);
            if (result == -1) return NotFound("Value Not Found");
            return Ok();
        }
        [HttpDelete]
        [Route("/api/ProductCategorySubCategoryValues/all")]
        public async Task<IActionResult> DeleteProductCategorySubCategoryValueAllAsync(int productId, int categoryId, int subCategoryId)
        {
            if (!await _productRepo.IsProductExistsAsync(productId))
                return NotFound("Product Not Found!");
            if (!await _categoryRepo.IsCategoryExistsAsync(categoryId))
                return NotFound("Category Not Found!");
            if (!await _categoryRepo.IsSubCategoryExistsAsync(subCategoryId))
                return NotFound("Sub Category Not Found!");
            var result = await _productRepo.DeleteProductCategorySubCategoryValueAll(productId, categoryId, subCategoryId);
            if (result == -1) return NotFound();
            return Ok();
        }
        [HttpPost]
        [Route("/api/products/pictures")]
        public async Task<IActionResult> UploadProductPicturesAsync([Required] ProductPictureDTO input)
        {
            if(!await _productRepo.IsProductExistsAsync(input.ProductId))
                return NotFound("Product Not Found ");
            if (input.Pictures.Any(p => p.Length > 5e6)) return BadRequest("Too Large Photo");
            foreach (var pic in input.Pictures)
            {
                var extension = Path.GetExtension(pic.FileName);
                List<string> validExtensions = new() { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                if (!validExtensions.Contains(extension))
                    return BadRequest("Invalid Extension!");
            }
            await _productRepo.AddProductPhotosAsync(input);
            return Created();
        }
        [HttpGet]
        [Route("/api/products/pictures")]
        public async Task<IActionResult> GetProductPicturesAsync(int productId)
        {
            if(!await _productRepo.IsProductExistsAsync(productId))
                return NotFound("Product Not Found!");
            var result = await _productRepo.GetProductPicturesAsync(productId);
            return Ok(result);
        }
        [HttpDelete]
        [Route("/api/products/pictures")]
        public async Task<IActionResult> RemoveProductPictureAsync(int productId, string ImageId)
        {
            if(!await _productRepo.IsProductExistsAsync(productId))
                return NotFound("Product Not Found!");
            if(!await _productRepo.IsProductImageExistsAsync(productId, ImageId))
                return NotFound("Image Not Found!");
            await _productRepo.RemoveProductPictureAsync(productId, ImageId);
            return Ok();
        }
        [HttpGet]
        [Route("/api/products/pagination")]
        public async Task<IActionResult> RenderPaginationAllAsync(int page, [Required] int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest();
            return Ok(await _productRepo.RenderPaginationForAllProductsAsync(page, pageSize));
        }
        [HttpOptions]
        [Route("/api/products/pagination")]
        public async Task<IActionResult> RenderPaginationSortedAsync(int page, int pageSize, string sortingIndex)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest();
            return Ok(await _productRepo.RenderSortedPaginationSortedAsync(page, pageSize, sortingIndex));
        }
        [HttpPut]
        [Route("/api/products/search")]
        public async Task<IActionResult> GetAllSearchResultsAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, int? OfferId)
        {
            return Ok(await _productRepo.GetAllProductsSearchAsync(Name, MinOriginalPrice, MaxOriginalPrice, MinAmount, MaxAmount, CategoriesIds, OfferId));
        }
        [HttpOptions]
        [Route("/api/products/search")]
        public async Task<IActionResult> GetAllSearchResultPaginatedAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, [Required] int page, [Required] int pageSize, int? offerId)
        {
            if (page <= 0 || pageSize <= 0) return BadRequest();
            return Ok(await _productRepo.GetAllProductsSearchPaginatedAsync(Name, MinOriginalPrice, MaxOriginalPrice, MinAmount, MaxAmount, CategoriesIds, page, pageSize, offerId));
        }
    }
}
