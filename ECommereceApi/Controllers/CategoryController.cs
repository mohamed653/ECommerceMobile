﻿using ECommereceApi.DTOs.Product;
using ECommereceApi.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IProductRepo _productRepo;
        public CategoryController(IProductRepo productRepo)
        {
            _productRepo = productRepo;
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
        [Route("/api/SubCategoryDetails/{subCategoryId:int}")]
        public async Task<IActionResult> GetSubCategoryDetailsAsync([Required] int subCategoryId)
        {
            if (!await _productRepo.IsSubCategoryExistsAsync(subCategoryId)) return NotFound("SubCategory Not Found");
            return Ok(await _productRepo.GetSubCategoryDetails(subCategoryId));
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
    }
}
