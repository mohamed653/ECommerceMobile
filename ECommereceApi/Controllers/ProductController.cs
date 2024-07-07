﻿using ECommereceApi.DTOs.Product;
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
        public ProductController(IProductRepo productRepo, ICartRepo cartRepo, IOfferRepo offerRepo, IOrderRepo orderRepo)
        {
            _productRepo = productRepo;
            _cartRepo = cartRepo;
            _offerRepo = offerRepo;
            _orderRepo = orderRepo;
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
            if (!await _productRepo.IsCategoryExistsAsync(product.CategoryId))
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
            var result = await _productRepo.DeleteProductAsync(id);
            if (result == Status.Failed) return BadRequest();
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync([Required] ProductAddDTO product, int Id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_productRepo.IsOrgignalPriceGreaterThanDiscount(product.OriginalPrice, product.Discount))
                return BadRequest("Price can't Be Equal or less than Discount!");
            if (!await _productRepo.IsCategoryExistsAsync(product.CategoryId))
                return NotFound("Category Not Found");
            await _productRepo.UpdateProductAsync(product, Id);
            return Ok();
        }
        [HttpGet]
        [Route("/api/product/{id:int}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var result = await _productRepo.GetProductDisplayDTOByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet]
        [Route("/api/category/{categoryId:int}")]
        public async Task<IActionResult> GetAllProductsByCategoryAsync(int categoryId)
        {
            var result = await _productRepo.GetAllCategoryProductsAsync(categoryId);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPost]
        [Route("/api/category")]
        public async Task<IActionResult> AddCategoryAsync(CategoryAddDTO category)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            return Created("", await _productRepo.AddCategoryAsync(category));
        }
        [HttpPut]
        [Route("/api/category")]
        public async Task<IActionResult> UpdateCategoryAsync(int id, CategoryAddDTO category)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (!await _productRepo.IsCategoryExistsAsync(id))
                return NotFound("Category Not Found!");
            var result = await _productRepo.UpdateCategoryAsync(id, category);
            if (result is null)
                return BadRequest();
            return Created("", result);
        }
        [HttpDelete]
        [Route("/api/category")]
        public async Task<IActionResult> DeleteCategoryAsync(int categoryId)
        {
            var result = await _productRepo.DeleteCategoryAsync(categoryId);
            if (result == Status.NotFound)
                return NotFound();
            return Ok();
        }
        [HttpGet]
        [Route("/api/category/all")]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            var result = await _productRepo.GetAllCategoriesAsync();
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet]
        [Route("/api/subCategory/all")]
        public async Task<IActionResult> GetAllSubCategoriesAsync()
        {
            return Ok(await _productRepo.GetAllSubCategoriesAsync());
        }
        [HttpGet]
        [Route("/api/subCategory/{id:int}")]
        public async Task<IActionResult> GetSubCategoryByIdAsync(int id)
        {
            var result = await _productRepo.GetSubCategoryById(id);
            if (result is null)
                return NotFound();
            return Ok(result);
        }
        [HttpPost]
        [Route("/api/subCategory")]
        public async Task<IActionResult> AddSubCategoryAsync(SubCategoryAddDTO category)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _productRepo.AddSubCategoryAsync(category);
            if (result is null)
                return NotFound();
            return Created("", result);
        }
        [HttpPut]
        [Route("/api/subCategory")]
        public async Task<IActionResult> UpdateSubCategoryAsync(int id, SubCategoryAddDTO cat)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _productRepo.UpdateSubCategoryAsync(id, cat);
            if (result is null)
                return NotFound();
            return Ok(result);
        }
        [HttpDelete]
        [Route("/api/subCategory")]
        public async Task<IActionResult> DeleteSubCategoryAsync(int subId)
        {
            var result = await _productRepo.DeleteSubCategoryAsync(subId);
            if (result == Status.NotFound)
                return NotFound();
            if (result == Status.Failed)
                return BadRequest("Sub Category is associated with other entities");
            return Ok();
        }
        [HttpGet]
        [Route("/api/subCategoryFromCategory/{categoryId:int}")]
        public async Task<IActionResult> GetAllSubCategoriesFromCategoryAsync(int categoryId)
        {
            var result = await _productRepo.GetAllSubCategoriesForCategoryAsync(categoryId);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet]
        [Route("/api/CategorySubCategoryValues/{ProductId:int}/{categoryId:int}/{subCategoryId:int}")]
        public async Task<IActionResult> GetCategorySubCategoryValuesAsync(int ProductId, int categoryId, int subCategoryId)
        {
            var result = await _productRepo.GetProductCategorySubCategoryValuesAsync(ProductId, categoryId, subCategoryId);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet]
        [Route("/api/CategorySubCategoryValuesAll/{ProductId:int}")]
        public async Task<IActionResult> GetAllCategorySubCategoryValuesAsync(int ProductId)
        {
            var result = await _productRepo.GetAllProductCategorySubCategoryValuesAsync(ProductId);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPost]
        [Route("/api/ProductCategorySubCategoryValues")]
        public async Task<IActionResult> AssignValueForProductCategorySubCategoryAsync(ProductCategorySubCategoyValueAddDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _productRepo.AssignValueForProductCategorySubCategory(input);
            if (result == null) return BadRequest();
            return Created("", result);
        }
        [HttpPost]
        [Route("/api/CategorySubCategoryValues")]
        public async Task<IActionResult> AddSubCategoryValue(CategorySubCategoryValuesAddDTO input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await _productRepo.IsCategoryExistsAsync(input.CategoryId))
                return NotFound("Category Not Found");
            if (!await _productRepo.IsSubCategoryExistsAsync(input.SubCategoryId))
                return NotFound("Sub Category Not Found");
            if (!await _productRepo.IsCategorySubCategoryExistsAsync(input.CategoryId, input.SubCategoryId))
                await _productRepo.AssignSubCategoryToCategoryAsync(input.CategoryId, input.SubCategoryId);
            var output = await _productRepo.AddSubCategoryValueAsync(input);
            if (output == null) return BadRequest();
            return Created("", output);
        }
        [HttpGet]
        [Route("/api/CategoryDetails/All")]
        public async Task<IActionResult> GetAllCategoriesDetailsAsync()
        {
            return Ok(await _productRepo.GetAllCategoriesDetailsAsync());
        }
        [HttpGet]
        [Route("/api/CategoryDetails/{categoryId:int}")]
        public async Task<IActionResult> GetCategoryDetailsAsync([Required] int categoryId)
        {
            if (!await _productRepo.IsCategoryExistsAsync(categoryId)) return NotFound("Category Not Found");
            return Ok(await _productRepo.GetCategoryDetailsAsync(categoryId));
        }
        [HttpGet]
        [Route("/api/Product/CategorySubCategoryValue")]
        public async Task<IActionResult> GetAllProductsForCategorySubCategoryValues([Required] int categoryId, [Required] int subCategoryId, [Required] string value)
        {
            if (value.IsNullOrEmpty()) return BadRequest("Invalid Value");
            if (!await _productRepo.IsCategoryExistsAsync(categoryId)) return NotFound("Category Not Found!");
            if (!await _productRepo.IsSubCategoryExistsAsync(subCategoryId)) return NotFound("Sub Category Not Found");
            int? categorySubCategoryId = await _productRepo.GetCategorySubCategoryIdFromSeparateIds(categoryId, subCategoryId);
            if (categorySubCategoryId is null) return NotFound("Category And Sub Category Not Related");
            return Ok(await _productRepo.GetProductsDisplayDTOsFromCategorySubCategoryIdAndValueAsync(categorySubCategoryId.Value, value));
        }
        [HttpGet]
        [Route("/api/SubCategoryDetails/{subCategoryId:int}")]
        public async Task<IActionResult> GetSubCategoryDetailsAsync([Required] int subCategoryId)
        {
            if (!await _productRepo.IsSubCategoryExistsAsync(subCategoryId)) return NotFound("SubCategory Not Found");
            return Ok(await _productRepo.GetSubCategoryDetails(subCategoryId));
        }
        [HttpDelete]
        [Route("/api/CategorySubCategoryValues")]
        public async Task<IActionResult> DeleteProductCategorySubCategoryValueAsync(int productId, int categoryId, int subCategoryId, string value)
        {
            var result = await _productRepo.DeleteProductCategorySubCategoryValue(productId, categoryId, subCategoryId, value);
            if (result == -1) return NotFound();
            return Ok();
        }
        [HttpDelete]
        [Route("/api/CategorySubCategoryValues/all")]
        public async Task<IActionResult> DeleteProductCategorySubCategoryValueAllAsync(int productId, int categoryId, int subCategoryId)
        {
            var result = await _productRepo.DeleteProductCategorySubCategoryValueAll(productId, categoryId, subCategoryId);
            if (result == -1) return NotFound();
            return Ok();
        }
        [HttpPatch]
        [Route("/api/CategorySubCategoryValues")]
        public async Task<IActionResult> UpdateCategorySubCategoryValueAsync(CategorySubCategoryValuesAddDTO input, string newValue)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _productRepo.UpdateCategorySubCategoryValue(input, newValue);
            if (result == null) return NotFound();
            return Ok(result);
        }
        //[HttpGet]
        //[Route("/api/subCategory/filtered")]
        //public async Task<IActionResult> GetAllProductsForSubCategoryAsync(int subCategoryId, string value)
        //{
        //    var result = await productRepo.GetAllProductsForSubCategoryAsync(subCategoryId, value);
        //    if (result == null) return NotFound();
        //    return Ok(result);
        //}
        [HttpPost]
        [Route("/api/products/pictures")]
        public async Task<IActionResult> UploadProductPicturesAsync([Required] ProductPictureDTO input)
        {
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
            var result = await _productRepo.GetProductPicturesAsync(productId);
            if (result is null)
                return NotFound();
            //string baseURL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            return Ok(result);
        }
        [HttpDelete]
        [Route("/api/products/pictures")]
        public async Task<IActionResult> RemoveProductPictureAsync(int productId, string url)
        {
            var result = await _productRepo.RemoveProductPictureAsync(productId, url);
            if (result == Status.NotFound) return NotFound();
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
