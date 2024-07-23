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
        /// <summary>
        /// Get Cart Products for a specific user
        /// </summary>
        /// <param name="id">user Id</param>
        /// <returns>
        /// NotFound if no user with this Id
        /// Ok with empty array if user has no products in cart
        /// Ok with array of Products if user has products in cart
        /// </returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetUserCartAsync(int id)
        {
            User user = await _repo.GetUserByIdAsync(id);
            if (user is null) return NotFound("مستخدم غير موجود");
            if (!_repo.IsUserHaveProductsInCart(user)) return Ok(Array.Empty<CartProductsDTO>());
            return Ok(await _repo.GetCartProductsAsync(user));
        }
        /// <summary>
        /// Delete all products from cart for a specific user
        /// </summary>
        /// <param name="id">user Id</param>
        /// <returns>
        /// NotFound if no user with this Id
        /// Ok if cart products deleted successfully
        /// </returns>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteUserCartAsync(int id)
        {
            User user = await _repo.GetUserByIdAsync(id);
            if(user is null) return NotFound("مستخدم غير موجود");
            await _repo.DeleteCartItemsAsync(user);
            return Ok();
        }
        /// <summary>
        /// Delete a specific product from cart for a specific user
        /// </summary>
        /// <param name="UserId">Id for user </param>
        /// <param name="ProductId">Id for product</param>
        /// <returns>
        /// NotFound if no user with this Id, no product with this Id or product is not in cart
        /// Ok if product deleted successfully
        /// </returns>
        [HttpDelete]
        [Route("{UserId:int}/{ProductId:int}")]
        public async Task<IActionResult> DeleteProductFromCartAsync(int UserId, int ProductId)
        {
            if(!await _repo.IsUserExistsByIdAsync(UserId)) return NotFound("مستخدم غير موجود");
            if(!await _repo.IsProductExistsByIdAsync(ProductId)) return NotFound("منتج غير موجود");
            if(!await _repo.IsProductExistsInCartAsync(ProductId, UserId)) return NotFound("هذا المنتج غير موجود في العربة");
            await _repo.DeleteProductFromCartAsync(UserId, ProductId);
            return Ok();
        }
        /// <summary>
        /// Add a product to cart for a specific user
        /// </summary>
        /// <param name="userId">Id for user</param>
        /// <param name="productId">Id for product</param>
        /// <param name="amount">Amount to be Added</param>
        /// <returns>
        /// NotFound if no user with this Id or no product with this Id
        /// BadRequest if amount is less than or equal to zero or amount is greater than product Availabe amount
        /// Ok if product added successfully
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddProductToCartAsync(int userId, int productId, int amount)
        {
            User user = await _repo.GetUserByIdAsync(userId);
            if(user is null) return NotFound("مستخدم غير موجود");
            var product = await _productRepo.GetProductDisplayDTOByIdAsync(productId);
            if(product is null) return NotFound("منتج غير موجود");
            if(amount <= 0) return BadRequest("كمية غير صالحة");
            if(amount > product.Amount) return BadRequest("كمية أكبر من الموجودة");
            await _repo.AddProductToCartAsync(user, product, amount);
            return Ok();
        }
        /// <summary>
        /// Add multiple products to cart for a specific user with different amounts
        /// </summary>
        /// <param name="cartProducts"></param>
        /// <returns>
        /// BadRequest if number of products and amounts are not equal.
        /// NotFound if no user with this Id or any product with this Id
        /// Ok with array of products in cart if products added successfully
        /// </returns>
        /// <remarks>
        /// if any product is not found in the database, it will return notfound and no product will be added to the cart
        /// </remarks>
        [HttpPut]
        public async Task<IActionResult> ClearAndAddMultipleProductsToCartAsync(MultipleProductsCardDTO cartProducts)
        {
            if(cartProducts.Amounts.Count != cartProducts.ProductsIds.Count || cartProducts.Amounts.Count == 0)
                return BadRequest("عدد المنتجات والكميات غير متساوي");
            User user = await _repo.GetUserByIdAsync(cartProducts.UserId);
            if(user is null) return NotFound("مستخدم غير موجود");
            if(!await _productRepo.IsAllProductsExistsAsync(cartProducts.ProductsIds)) return NotFound("منتج من المنتجات غير موجود");
            await _repo.DeleteCartItemsAsync(user);
            await _repo.AddProductsToCartAsync(user, cartProducts.ProductsIds.ToList(), cartProducts.Amounts);
            return Ok(await _repo.GetCartProductsAsync(user));
        }
        /// <summary>
        /// Update Product quantity in cart for a specific user
        /// </summary>
        /// <param name="UserId">Id for user</param>
        /// <param name="ProductId">Id for Product</param>
        /// <param name="NewQuantity">New value for quantity</param>
        /// <returns>
        /// NotFound if no user with this Id, no product with this Id or product is not in cart
        /// BadRequest if NewQuantity is less than or equal to zero or NewQuantity is greater than product Availabe amount
        /// Ok with array of products in cart if quantity updated successfully
        /// </returns>
        [HttpPatch]
        public async Task<IActionResult> UpdateProductQuantityAsync(int UserId, int ProductId, int NewQuantity)
        {
            if(!await _repo.IsUserExistsByIdAsync(UserId)) return NotFound("مستخدم غير موجود");
            if(!await _repo.IsProductExistsByIdAsync(ProductId)) return NotFound("منتج من المنتجات غير موجود");
            if(!await _repo.IsProductExistsInCartAsync(ProductId, UserId)) return NotFound("منتج غير موجود بالعربة");
            if(!await _repo.IsProductQuantityAvailableinStockAsync(ProductId, NewQuantity)) return BadRequest("الكمية المطلوبة غير متاحة");
            await _repo.UpdateProductQuantityInCartAsync(UserId, ProductId, NewQuantity);
            var user = await _repo.GetUserByIdAsync(UserId);
            return Ok(await _repo.GetCartProductsAsync(user));
        }
    }
}
