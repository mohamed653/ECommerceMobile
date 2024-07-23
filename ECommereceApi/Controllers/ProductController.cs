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
        /// <summary>
        /// Get All Products in DataBase
        /// </summary>
        /// <remarks>
        /// Products that are deleted are not included in the result
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            return Ok(await _productRepo.GetAllProductDisplayDTOsAsync());
        }
        /// <summary>
        /// Add Product To DataBase
        /// </summary>
        /// <param name="product"></param>
        /// <returns>
        /// BadRequest if price is less than or equal to discount or model state is not valid
        /// NotFound if Category Not Found
        /// Created if Product Added Successfully
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddProductAsync([Required] ProductAddDTO product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_productRepo.IsOrgignalPriceGreaterThanDiscount(product.OriginalPrice, product.Discount))
                return BadRequest("السعر لايمكن ان يكون اكبر من او يساوي الخصم");
            if (!await _categoryRepo.IsCategoryExistsAsync(product.CategoryId))
                return NotFound("قسم اساسي غير موجود");
            var result = await _productRepo.AddProductAsync(product);
            if (result is null)
                return BadRequest();
            return Created("", result);
        }
        /// <summary>
        /// Mark product as Deleted in Db
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns>
        /// NotFound if Product Not Found
        /// BadRequest if Product Is In Cart Or In Active Or Coming Offer Or In Active Order
        /// </returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            if (!await _productRepo.IsProductExistsAsync(id)) return NotFound("منتج غير موجود");
            if (await _cartRepo.IsProductExistsInAnyCartAsync(id))
                return BadRequest("منتج موجود في عربة");
            if (await _offerRepo.IsProductInActiveOrComingOfferAsync(id))
                return BadRequest("منتج في عرض حالي او قادم");
            if (await _orderRepo.IsProductInActiveOrderAsync(id))
                return BadRequest("منتج في طلب ساري");
            await _productRepo.DeleteProductAsync(id);
            return Ok();
        }
        /// <summary>
        /// Update Product In DataBase
        /// </summary>
        /// <param name="product">new Product Data</param>
        /// <param name="Id">product Id</param>
        /// <returns>
        /// BadRequest if Modelstate is not valid or price is less than or equal to discount
        /// NotFount if Product Not Found or Category Not Found
        /// Ok if Product Updated Successfully
        /// </returns>
        /// <remarks>
        /// product object must be provided in the body of the request
        /// </remarks>
        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync([Required] ProductAddDTO product, int Id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await _productRepo.IsProductExistsAsync(Id))
                return NotFound("منتج غير موجود");
            if (!_productRepo.IsOrgignalPriceGreaterThanDiscount(product.OriginalPrice, product.Discount))
                return BadRequest("السعر لايمكن ان يكون اكبر من او يساوي الخصم");
            if (!await _categoryRepo.IsCategoryExistsAsync(product.CategoryId))
                return NotFound("قسم اساسي غير موجود");
            await _productRepo.UpdateProductAsync(product, Id);
            return Ok();
        }
        /// <summary>
        /// Get Product By Id
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns>
        /// NotFound if Product Not Found
        /// Ok if Product Found and returned successfully
        /// </returns>
        [HttpGet]
        [Route("/api/product/{id:int}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            if (!await _productRepo.IsProductExistsAsync(id))
                return NotFound("منتج غير موجود");
            var result = await _productRepo.GetProductDisplayDTOByIdAsync(id);
            return Ok(result);
        }
        /// <summary>
        /// Get All Products In Category By Category Id
        /// </summary>
        /// <param name="categoryId">Id Of Category</param>
        /// <returns>
        /// NotFound if Category Not Found
        /// Ok if Category Found and returned successfully
        /// </returns>
        [HttpGet]
        [Route("/api/category/{categoryId:int}")]
        public async Task<IActionResult> GetAllProductsByCategoryAsync(int categoryId)
        {
            if (!await _categoryRepo.IsCategoryExistsAsync(categoryId))
                return NotFound("قسم اساسي غير موجود");
            var result = await _productRepo.GetAllCategoryProductsAsync(categoryId);
            return Ok(result);
        }
        /// <summary>
        /// Get All Products In specific category, specific subcategory and specific value
        /// </summary>
        /// <param name="categoryId">Id Of Category</param>
        /// <param name="subCategoryId">Id Of SubCategory</param>
        /// <param name="value">Value assigned to them</param>
        /// <returns>
        /// BadRequest if Value is null or empty
        /// NotFound if Category Not Found or SubCategory Not Found or Category And SubCategory Not Related
        /// Ok if Category And SubCategory Found and returned result successfully
        /// </returns>
        [HttpGet]
        [Route("/api/Product/CategorySubCategoryValue")]
        public async Task<IActionResult> GetAllProductsForCategorySubCategoryValues([Required] int categoryId, [Required] int subCategoryId, [Required] string value)
        {
            if (value.IsNullOrEmpty()) return BadRequest("قيمة غير صحيحة");
            if (!await _categoryRepo.IsCategoryExistsAsync(categoryId)) return NotFound("قسم اساسي غير موجود");
            if (!await _categoryRepo.IsSubCategoryExistsAsync(subCategoryId)) return NotFound("قسم فرعي غير موجود");
            int? categorySubCategoryId = await _categoryRepo.GetCategorySubCategoryIdFromSeparateIds(categoryId, subCategoryId);
            if (categorySubCategoryId is null) return NotFound("قسم اساسي وفرعي غير مرتبطين");
            return Ok(await _productRepo.GetProductsDisplayDTOsFromCategorySubCategoryIdAndValueAsync(categorySubCategoryId.Value, value));
        }
        /// <summary>
        /// Remove value From product category subCategory
        /// </summary>
        /// <param name="productId">Id of product</param>
        /// <param name="categoryId">Id of Category</param>
        /// <param name="subCategoryId">Id of subCategory</param>
        /// <param name="value">Value to be removed</param>
        /// <returns>
        /// NotFound if Product Not Found or Category Not Found or SubCategory Not Found or Category And SubCategory Not Related or Value Not Found
        /// Ok if Value Removed Successfully
        /// </returns>
        [HttpDelete]
        [Route("/api/ProductCategorySubCategoryValues")]
        public async Task<IActionResult> DeleteProductCategorySubCategoryValueAsync(int productId, int categoryId, int subCategoryId, string value)
        {
            if (!await _productRepo.IsProductExistsAsync(productId))
                return NotFound("منتج غير موجود");
            if (!await _categoryRepo.IsCategoryExistsAsync(categoryId))
                return NotFound("قسم اساسي غير موجود");
            if (!await _categoryRepo.IsSubCategoryExistsAsync(subCategoryId))
                return NotFound("قسم فرعي غير موجود");
            int? categorySubCategoryId = await _categoryRepo.GetCategorySubCategoryIdFromSeparateIds(categoryId, subCategoryId);
            if (categorySubCategoryId is null) return NotFound("قسم اساسي وفرعي غير مرتبطين");
            var result = await _productRepo.DeleteProductCategorySubCategoryValue(productId, categoryId, subCategoryId, value);
            if (result == -1) return NotFound("قيمة غير موجودة");
            return Ok();
        }
        /// <summary>
        /// Remove All Values From Category SubCategory For Product
        /// </summary>
        /// <param name="productId">Id of Product</param>
        /// <param name="categoryId">Id of Category</param>
        /// <param name="subCategoryId">Id of SubCategory</param>
        /// <returns>
        /// Not Found if Product Not Found or Category Not Found or SubCategory Not Found
        /// Ok if All Values Removed Successfully
        /// </returns>
        [HttpDelete]
        [Route("/api/ProductCategorySubCategoryValues/all")]
        public async Task<IActionResult> DeleteProductCategorySubCategoryValueAllAsync(int productId, int categoryId, int subCategoryId)
        {
            if (!await _productRepo.IsProductExistsAsync(productId))
                return NotFound("منتج غير موجود");
            if (!await _categoryRepo.IsCategoryExistsAsync(categoryId))
                return NotFound("قسم اساسي غير موجود");
            if (!await _categoryRepo.IsSubCategoryExistsAsync(subCategoryId))
                return NotFound("قسم فرعي غير موجود");
            var result = await _productRepo.DeleteProductCategorySubCategoryValueAll(productId, categoryId, subCategoryId);
            if (result == -1) return NotFound();
            return Ok();
        }
        /// <summary>
        /// Upload Product Pictures
        /// </summary>
        /// <param name="input"></param>
        /// <returns>
        /// NotFound if Product Not Found
        /// BadRequest if Picture Size is Too Large or Invalid Extension
        /// Created if Pictures Uploaded Successfully
        /// </returns>
        /// <remarks>
        /// If any picture size is greater than 5MB, it will return BadRequest
        /// valid Extenstions are .jpg, .jpeg, .png, .gif, .bmp, if any picture has invalid extension, it will return BadRequest
        /// returned BadRequest mean that no picture was uploaded
        /// </remarks>
        [HttpPost]
        [Route("/api/products/pictures")]
        public async Task<IActionResult> UploadProductPicturesAsync([Required] ProductPictureDTO input)
        {
            if(!await _productRepo.IsProductExistsAsync(input.ProductId))
                return NotFound("منتج غير موجود");
            if (input.Pictures.Any(p => p.Length > 5e6)) return BadRequest("صورة تجاوزت الحد المسموح به");
            foreach (var pic in input.Pictures)
            {
                var extension = Path.GetExtension(pic.FileName);
                List<string> validExtensions = new() { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                if (!validExtensions.Contains(extension))
                    return BadRequest("صيغة غير مدعومة");
            }
            await _productRepo.AddProductPhotosAsync(input);
            return Created();
        }
        /// <summary>
        /// Get All Product Pictures
        /// </summary>
        /// <param name="productId">Id of product</param>
        /// <returns>
        /// NotFound if Product Not Found
        /// Ok if Pictures Found and returned successfully
        /// </returns>
        [HttpGet]
        [Route("/api/products/pictures")]
        public async Task<IActionResult> GetProductPicturesAsync(int productId)
        {
            if(!await _productRepo.IsProductExistsAsync(productId))
                return NotFound("منتج غير موجود");
            var result = await _productRepo.GetProductPicturesAsync(productId);
            return Ok(result);
        }
        /// <summary>
        /// Remove Product Picture
        /// </summary>
        /// <param name="productId">Id of product</param>
        /// <param name="ImageId">Id of Image</param>
        /// <returns>
        /// NotFound if Product Not Found or Image Not Found
        /// Ok if Image Removed Successfully
        /// </returns>
        [HttpDelete]
        [Route("/api/products/pictures")]
        public async Task<IActionResult> RemoveProductPictureAsync(int productId, string ImageId)
        {
            if(!await _productRepo.IsProductExistsAsync(productId))
                return NotFound("منتج غير موجود");
            if(!await _productRepo.IsProductImageExistsAsync(productId, ImageId))
                return NotFound("صورة غير موجودة");
            await _productRepo.RemoveProductPictureAsync(productId, ImageId);
            return Ok();
        }
        /// <summary>
        /// Get All Products Paginated
        /// </summary>
        /// <param name="page">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>
        /// BadRequest if page or pageSize is less than or equal to 0
        /// Ok if results returned successfully
        /// </returns>
        [HttpGet]
        [Route("/api/products/pagination")]
        public async Task<IActionResult> RenderPaginationAllAsync(int page, [Required] int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("رقم وحجم الصفحة لايمكن ان يكونوا اقل من او يساوي 0");
            return Ok(await _productRepo.RenderPaginationForAllProductsAsync(page, pageSize));
        }
        /// <summary>
        /// Get All Products Paginated And Sorted
        /// </summary>
        /// <param name="page">page Number</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="sortingIndex">index to sort by</param>
        /// <returns>
        /// BadRequest if page or pageSize is less than or equal to 0
        /// Ok if results returned successfully
        /// </returns>
        /// <remarks>
        /// Available Sorting Indices are:
        /// name_asc, name_des, price_asc, price_des, amount_asc, amount_des, discount_asc, discount_des
        /// </remarks>
        [HttpOptions]
        [Route("/api/products/pagination")]
        public async Task<IActionResult> RenderPaginationSortedAsync(int page, int pageSize, string sortingIndex)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("رقم وحجم الصفحة لايمكن ان يكونوا اقل من او يساوي 0");
            return Ok(await _productRepo.RenderSortedPaginationSortedAsync(page, pageSize, sortingIndex));
        }
        /// <summary>
        /// Get Products Search Results
        /// </summary>
        /// <param name="Name">Product Name</param>
        /// <param name="MinOriginalPrice">Minimum Original Price</param>
        /// <param name="MaxOriginalPrice">Maximum original Price</param>
        /// <param name="MinAmount">Minimum Amount</param>
        /// <param name="MaxAmount">Maximum Amount</param>
        /// <param name="CategoriesIds">Categories Ids</param>
        /// <param name="OfferId">Offer Id</param>
        /// <returns>
        /// Search Results
        /// </returns>
        /// <remarks>
        /// Parameters doesn't have to be provided, if not provided, it will be ignored
        /// </remarks>
        [HttpPut]
        [Route("/api/products/search")]
        public async Task<IActionResult> GetAllSearchResultsAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, int? OfferId)
        {
            return Ok(await _productRepo.GetAllProductsSearchAsync(Name, MinOriginalPrice, MaxOriginalPrice, MinAmount, MaxAmount, CategoriesIds, OfferId));
        }
        /// <summary>
        /// Get All Search Results Paginated
        /// </summary>
        /// <param name="Name">Product Name</param>
        /// <param name="MinOriginalPrice">Minimum Original Price</param>
        /// <param name="MaxOriginalPrice">Maximum original Price</param>
        /// <param name="MinAmount">Minimum Amount</param>
        /// <param name="MaxAmount">Maximum Amount</param>
        /// <param name="CategoriesIds">Categories Ids</param>
        /// <param name="page">page Number</param>
        /// <param name="pageSize">size of one Page</param>
        /// <param name="offerId">Id of Offer</param>
        /// <returns></returns>
        [HttpOptions]
        [Route("/api/products/search")]
        public async Task<IActionResult> GetAllSearchResultPaginatedAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, [Required] int page, [Required] int pageSize, int? offerId)
        {
            if (page <= 0 || pageSize <= 0) return BadRequest("رقم وحجم الصفحة لايمكن ان يكونوا اقل من او يساوي 0");
            return Ok(await _productRepo.GetAllProductsSearchPaginatedAsync(Name, MinOriginalPrice, MaxOriginalPrice, MinAmount, MaxAmount, CategoriesIds, page, pageSize, offerId));
        }
    }
}
