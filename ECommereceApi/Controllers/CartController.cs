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
        [Route("{id:int}")]
        public async Task<IActionResult> GetUserCartAsync(int id)
        {
            User user = await _repo.GetUserByIdAsync(id);
            if (user is null) return BadRequest("No user with this Id");
            if (!_repo.IsUserHaveProductsInCart(user)) return Ok(Array.Empty<CartProductsDTO>());
            return Ok(await _repo.GetCartProductsAsync(user));
        }
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteUserCartAsync(int id)
        {
            User user = await _repo.GetUserByIdAsync(id);
            if(user is null) return BadRequest("No user with this Id");
            await _repo.DeleteCartItemsAsync(user);
            return Ok();
        }
        [HttpDelete]
        [Route("{UserId:int}/{ProductId:int}")]
        public async Task<IActionResult> DeleteProductFromCartAsync(int UserId, int ProductId)
        {
            if(!await _repo.IsUserExistsByIdAsync(UserId)) return NotFound("No User With this Id");
            if(!await _repo.IsProductExistsByIdAsync(ProductId)) return NotFound("No Product with this Id");
            if(!await _repo.IsProductExistsInCartAsync(ProductId, UserId)) return NotFound("Product Is not in cart");
            await _repo.DeleteProductFromCartAsync(UserId, ProductId);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> AddProductToCartAsync(int userId, int productId, int amount)
        {
            User user = await _repo.GetUserByIdAsync(userId);
            if(user is null) return BadRequest("No user with this Id");
            var product = await _productRepo.GetProductDisplayDTOByIdAsync(productId);
            if(product is null) return BadRequest("No product with this Id");
            if(amount <= 0) return BadRequest("Invalid Amount Value!");
            if(amount > product.Amount) return BadRequest("There Are no Enough Amount");
            await _repo.AddProductToCartAsync(user, product, amount);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> ClearAndAddMultipleProductsToCartAsync(MultipleProductsCardDTO cartProducts)
        {
            if(cartProducts.Amounts.Count != cartProducts.ProductsIds.Count || cartProducts.Amounts.Count == 0)
                return BadRequest("Invalid number of items");
            User user = await _repo.GetUserByIdAsync(cartProducts.UserId);
            if(user is null) return BadRequest("No user with this Id");
            if(!await _productRepo.IsAllProductsExistsAsync(cartProducts.ProductsIds)) return BadRequest("invalid Products Ids");
            await _repo.DeleteCartItemsAsync(user);
            await _repo.AddProductsToCartAsync(user, cartProducts.ProductsIds.ToList(), cartProducts.Amounts);
            return Ok(await _repo.GetCartProductsAsync(user));
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateProductQuantityAsync(int UserId, int ProductId, int NewQuantity)
        {
            if(!await _repo.IsUserExistsByIdAsync(UserId)) return NotFound("No User With this Id");
            if(!await _repo.IsProductExistsByIdAsync(ProductId)) return NotFound("No Product with this Id");
            if(!await _repo.IsProductExistsInCartAsync(ProductId, UserId)) return NotFound("Product Is not in cart");
            if(!await _repo.IsProductQuantityAvailableinStockAsync(ProductId, NewQuantity)) return BadRequest("Required Quantity Not Available in Stock");
            await _repo.UpdateProductQuantityInCartAsync(UserId, ProductId, NewQuantity);
            var user = await _repo.GetUserByIdAsync(UserId);
            return Ok(await _repo.GetCartProductsAsync(user));
        }
    }
}
