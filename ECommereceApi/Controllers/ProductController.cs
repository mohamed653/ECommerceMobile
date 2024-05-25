using ECommereceApi.DTOs;
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
        private readonly IProductRepo productRepo;
        public ProductController(IProductRepo _productRepo)
        {
            productRepo = _productRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            return Ok(await productRepo.GetAllProductsAsync());
        }
        [HttpPost]
        public async Task<IActionResult> AddProductAsync([Required]ProductAddDTO product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await productRepo.AddProductAsync(product);
            return Created("", result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var result = await productRepo.DeleteProductAsync(id);
            if (result == Status.Failed) return BadRequest();
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync([Required]ProductAddDTO product, int Id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await productRepo.UpdateProductAsync(product, Id);
            if (result == Status.NotFound) return NotFound();
            return Ok();
        }
        [HttpGet]
        [Route("/api/{id:int}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var result = await productRepo.GetProductByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet]
        [Route("/api/category/{categoryId:int}")]
        public async Task<IActionResult> GetAllProductsByCategoryAsync(int categoryId)
        {
            var result = await productRepo.GetAllCategoryProductsAsync(categoryId);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet]
        [Route("/api/category/all")]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            var result = await productRepo.GetAllCategoriesAsync();
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet]
        [Route("/api/subCategory/{categoryId:int}")]
        public async Task<IActionResult> GetAllSubCategoriesFromCategoryAsync(int categoryId)
        {
            var result = await productRepo.GetAllSubCategoriesForCategoryAsync(categoryId);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet]
        [Route("/api/subCategory/filtered")]
        public async Task<IActionResult> GetAllProductsForSubCategoryAsync(int subCategoryId, string value)
        {
            var result = await productRepo.GetAllProductsForSubCategoryAsync(subCategoryId, value);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPost]
        [Route("/api/products/pictures")]
        public async Task<IActionResult> UploadProductPicturesAsync([Required]ProductPictureDTO input)
        {
            if (input.Pictures.Any(p => p.Length > 5e6)) return BadRequest("Too Large Photo");
            foreach (var pic in input.Pictures)
            {
                var extension = Path.GetExtension(pic.FileName);
                List<string> validExtensions = new() { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                if (!validExtensions.Contains(extension))
                    return BadRequest("Invalid Extension!");
            }
            await productRepo.AddProductPhotosAsync(input);
            return Created();
        }
        [HttpGet]
        [Route("/api/products/pictures")]
        public async Task<IActionResult> GetProductPicturesAsync(int productId)
        {
            var result = await productRepo.GetProductPicturesAsync(productId);
            if (result is null)
                return NotFound();
            //string baseURL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            return Ok(result);
        }
        [HttpDelete]
        [Route("/api/products/pictures")]
        public async Task<IActionResult> RemoveProductPictureAsync(int productId, string url)
        {
            var result = await productRepo.RemoveProductPictureAsync(productId, url);
            if (result == Status.NotFound) return NotFound();
            return Ok();
        }
        [HttpGet]
        [Route("/api/products/pagination")]
        public async Task<IActionResult> RenderPaginationAllAsync(int page, [Required]int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest();
            return Ok(await productRepo.RenderPaginationForAllProductsAsync(page, pageSize));
        }
        [HttpOptions]
        [Route("/api/products/pagination")]
        public async Task<IActionResult> RenderPaginationSortedAsync(int page, int pageSize, string sortingIndex)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest();
            return Ok(await productRepo.RenderSortedPaginationSortedAsync(page, pageSize, sortingIndex));
        }
        [HttpPut]
        [Route("/api/products/search")]
        public async Task<IActionResult> GetAllSearchResultsAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds)
        {
            return Ok(await productRepo.GetAllProductsSearchAsync(Name, MinOriginalPrice, MaxOriginalPrice, MinAmount, MaxAmount, CategoriesIds));
        }
        [HttpOptions]
        [Route("/api/products/search")]
        public async Task<IActionResult> GetAllSearchResultPaginatedAsync(string? Name, double? MinOriginalPrice, double? MaxOriginalPrice, int? MinAmount, int? MaxAmount, List<int>? CategoriesIds, [Required]int page, [Required]int pageSize)
        {
            if(page <= 0 || pageSize <= 0) return BadRequest();
            return Ok(await productRepo.GetAllProductsSearchPaginatedAsync(Name, MinOriginalPrice, MaxOriginalPrice, MinAmount, MaxAmount, CategoriesIds, page, pageSize));
        }
    }
}
