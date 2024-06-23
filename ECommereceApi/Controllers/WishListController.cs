using ECommereceApi.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishListRepo _wishListRepo;
        private readonly IProductRepo _productRepo;
        public WishListController(IWishListRepo wishListRepo, IProductRepo productRepo)
        {
            _wishListRepo = wishListRepo;
            _productRepo = productRepo;

        }
        [HttpPost]
        public async Task<IActionResult> AddWishList(WishListDTO wishListDTO)
        {
            
            await _wishListRepo.AddWishList(wishListDTO);
            return Ok();

        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWishListProductsByUserId(int userId)
        {
            var products = await _wishListRepo.GetWishListProducts(userId);
            if (products.Count == 0)
            {
                return Ok("No product found in your wishlist");
            }
            return Ok(products);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWishList(int id)
        {
            var result = await _wishListRepo.DeleteWishList(id);
            if (result == Status.NotFound)
            {
                return NotFound();
            }
            else if(result == Status.Deleted)
                return Ok();
           return BadRequest();
        }
        [HttpDelete("{userId}/{productId}")]
        public async Task<IActionResult> DeleteWishListItem(int userId, int productId)
        {
            var wishList = await _wishListRepo.DeleteWishListItem(userId, productId);
            if (wishList == null)
            {
                return NotFound();
            }
            return Ok(wishList);
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopWishlistedProducts()
        {
            var products = await _wishListRepo.GetTopWishlistedProducts();
            if (products.Count == 0)
            {
                return Ok("No product found in wishlist");
            }
            return Ok(products);
        }
    }
}
