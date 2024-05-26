using ECommereceApi.DTOs.Product;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepo _repo;
        private readonly IProductRepo _productRepo;

        public CartController(ICartRepo repo, IProductRepo productRepo)
        {
            _repo = repo;
            _productRepo = productRepo;
        }
        [HttpGet]
        [Route("/{id:int}")]
        public async Task<IActionResult> GetUserCart(int id)
        {
            User user = await _repo.GetUserByIdAsync(id);
            if (user is null)
            {
                return BadRequest("No user with this Id");
            }
            return Ok(await _repo.GetCartProductsAsync(user));
        }
        [HttpDelete]
        [Route("/{id:int}")]
        public async Task<IActionResult> DeleteUserCart(int id)
        {
            User user = await _repo.GetUserByIdAsync(id);
            if(user is null)
                return BadRequest("No user with this Id");
            _repo.DeleteCartItemsAsync(user);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> AddProductToCart(int userId, int productId, int amount)
        {
            User user = await _repo.GetUserByIdAsync(userId);
            if(user is null)
                return BadRequest("No user with this Id");
            var product = await _productRepo.GetProductByIdAsync(productId);
            if(product is null)
                return BadRequest("No product with this Id");
            _repo.AddProductToCartAsync(user, product, amount);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> ClearAndAddMultipleProductsToCart(MultipleProductsCardDTO cartProducts)
        {
            if(cartProducts.Amounts.Count != cartProducts.ProductsIds.Count)
                return BadRequest("Invalid number of items");
            User user = await _repo.GetUserByIdAsync(cartProducts.UserId);
            if(user is null)
                return BadRequest("No user with this Id");
            if(!await _productRepo.IsAllProductsExistsAsync(cartProducts.ProductsIds))
                return BadRequest("invalid Products Ids");
            _repo.ClearAndAddMultipleProducts(user, cartProducts.ProductsIds.ToList(), cartProducts.Amounts);
            return Ok();
        }

    }
}
