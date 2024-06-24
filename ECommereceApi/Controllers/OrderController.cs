using ECommereceApi.DTOs.Order;
using Microsoft.AspNetCore.Mvc;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderRepo _orderRepo;
        private readonly ICartRepo _cartRepo;
        public OrderController(IOrderRepo orderRepo, ICartRepo cartRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
        }
        [HttpGet]
        public async Task<IActionResult> PreviewOrder(int userId)
        {
            var user = await _cartRepo.GetUserByIdAsync(userId);
            if (user is null)
            {
                return NotFound("user doesn't have cart / not exist");
            }
            var cartProductsDTO = await _cartRepo.GetCartProductsAsync(user);
            if(!await _orderRepo.IsAllCartItemsAvailableAsync(cartProductsDTO))
            {
                return BadRequest("some products are not available");
            }
            return Ok(await _orderRepo.GetOrderPreviewAsync(cartProductsDTO));
        }
        [HttpPost]
        [Route("ConfirmWithoutOffer")]
        public async Task<IActionResult> ConfirmOrderWithoutOffer([FromBody] AddOrderWithoutOfferDTO addOrderWithoutOfferDTO)
        {
            var user = await _cartRepo.GetUserByIdAsync(addOrderWithoutOfferDTO.UserId);
            if (user is null)
            {
                return NotFound("user doesn't have cart / not exist");
            }
            var cartProductsDTO = await _cartRepo.GetCartProductsAsync(user);
            if (!await _orderRepo.IsAllCartItemsAvailableAsync(cartProductsDTO))
            {
                return BadRequest("some products are not available");
            }
            //var order = await _orderRepo.AddOrderWithoutOfferAsync(cartProductsDTO, addOrderWithoutOfferDTO);
            return Ok(null);
            //return Ok(order);
        }
    }
}
