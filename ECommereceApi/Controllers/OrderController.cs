using ECommereceApi.DTOs.Order;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Enums;
using ECommereceApi.Repo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

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
            if (!await _orderRepo.IsAllCartItemsAvailableAsync(cartProductsDTO))
            {
                return BadRequest("some products are not available");
            }
            return Ok(await _orderRepo.GetOrderPreviewAsync(cartProductsDTO));
        }

        // **************************************** Hamed **************************************** 

        [HttpGet]
        [Route("GetUserOrdersPaginated")]
        public async Task<IActionResult> GetUserOrdersPaginated(int userId, int page, [Required] int pageSize)
        {
            var user = await _cartRepo.GetUserByIdAsync(userId);
            if (user is null)
            {
                return NotFound("No User Found!");
            }
            if (page <= 0 || pageSize <= 0)
                return BadRequest();

            var orders = await _orderRepo.GetUserOrdersPaginatedAsync(userId, page, pageSize);
            if (orders.Items.Count > 0)
                return Ok(orders);

            return Ok("No orders found for this user");
        }

        /// <summary>
        ///  Status: 0 => Pending,1==> Accepted, 2 => Shipped, 3 => Delivered, 4 => Cancelled
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderStatus"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserOrdersByStatusPaginated")]
        public async Task<IActionResult> GetUserOrdersByStatusPaginated(int userId,OrderStatus orderStatus, int page, [Required] int pageSize)
        {
            var user = await _cartRepo.GetUserByIdAsync(userId);
            if (user is null)
            {
                return NotFound("No User Found!");
            }
            if (page <= 0 || pageSize <= 0)
                return BadRequest();

            var orders = await _orderRepo.GetUserOrdersByStatusPaginatedAsync(userId, orderStatus ,page, pageSize);
            if (orders.Items.Count > 0)
                return Ok(orders);

            return Ok("No orders found for this user");
        }
        [HttpGet]
        [Route("GetAllOrdersPaginated")]
        public async Task<IActionResult> GetAllOrdersPaginated(int page, [Required] int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest();

            var orders = await _orderRepo.GetAllOrdersPaginatedAsync( page, pageSize);
            if (orders.Items.Count > 0)
                return Ok(orders);

            return Ok("No orders found for this user");
        }

        [HttpGet]
        [Route("GetOrdersByStatusPaginated")]
        public async Task<IActionResult> GetOrdersByStatusPaginated(OrderStatus orderStatus,int page, [Required] int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest();

            var orders = await _orderRepo.GetOrdersByStatusPaginatedAsync(orderStatus, page, pageSize);
            if (orders.Items.Count > 0)
                return Ok(orders);

            return Ok("No orders found for this user");
        }

        [HttpGet]
        [Route("GetOrderById")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound("No Order Found!");
            }
            return Ok(order);
        }

        [HttpGet]
        [Route("GetOrderStats")]
        public async Task<IActionResult> GetOrderStats()
        {
            var stats = await _orderRepo.GetOrderStats();
            return Ok(stats);
        }
        [HttpGet]
        [Route("GetOrderStatsByUserId")]
        public async Task<IActionResult> GetOrderStatsByUserId(int userId)
        {
            var user = await _cartRepo.GetUserByIdAsync(userId);
            if (user is null)
            {
                return NotFound("No User Found!");
            }
            var stats = await _orderRepo.GetUserOrderStats(userId);
            return Ok(stats);
        }

        // Calculated The Final Total Price Of The Order
        [HttpGet]
        [Route("GetFinalPriceDetails/{offerId}/{userId}")]
        public async Task<IActionResult> GetFinalPriceDetails(int offerId, int userId)
        {
            try
            {
                var final = await _orderRepo.GetFinalOfferPriceAsync(offerId, userId);
                return Ok(new { cartFinalPrice= final.Item1, offerFinalPrice =final.Item2});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }



        [HttpPost]
        [Route("ConfirmOrder")] // ** Updated By Hamed**
        public async Task<IActionResult> ConfirmOrder([FromBody] AddOrderOfferDTO addOrderOfferDTO)
        {
            var user = await _cartRepo.GetUserByIdAsync(addOrderOfferDTO.UserId);
            if (user is null)
            {
                return NotFound("user doesn't have cart / not exist");
            }
            var cartProductsDTO = await _cartRepo.GetCartProductsAsync(user);
            if (!await _orderRepo.IsAllCartItemsAvailableAsync(cartProductsDTO))
            {
                return BadRequest("some products are not available");
            }
            try
            {
                var orderId = await _orderRepo.ConfirmOrder(addOrderOfferDTO);
                return Ok(new {OrderId = orderId});
            }
            catch (Exception ex)
            {
                return BadRequest("Order Operation Failed");
                throw;
            }
        
        }

        [HttpPost]
        [Route("ChangeStatusAccepted")]
        public async Task<IActionResult> ChangeStatusAccepted(Guid orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound("order not found");
            }
            if (order.Status != OrderStatus.Pending)
            {
                return BadRequest("order is not in Pending state");
            }
            try
            {
                await _orderRepo.ChangeOrderStatusAsync(orderId, OrderStatus.Accepted);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }

            return Ok();
        }

        [HttpPost]
        [Route("ChangeStatusShipped")]
        public async Task<IActionResult> ChangeStatusShipped(Guid orderId,int arrivalInDays)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound("order not found");
            }
            if (arrivalInDays <= 0)
                return BadRequest("arrival days must be greater than 0");
            else if(arrivalInDays > 14)
                return BadRequest("arrival days must be less than 14");

            if (order.Status != OrderStatus.Accepted)
            {
                return BadRequest("order is not in Accepted state");
            }
            try
            {
                await _orderRepo.ChangeOrderStatusAsync(orderId, OrderStatus.Shipped, arrivalInDays);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
          
            return Ok();
        }
        [HttpPost]
        [Route("ChangeStatusDelivered")]
        public async Task<IActionResult> ChangeStatusDelivered(Guid orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound("order not found");
            }
            if (order.Status != OrderStatus.Shipped)
            {
                return BadRequest("order is not in Shipped state");
            }
            try
            {
                await _orderRepo.ChangeOrderStatusAsync(orderId, OrderStatus.Delivered);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
        [HttpPost]
        [Route("ChangeStatusCancelled")]
        public async Task<IActionResult> ChangeStatusCancelled(Guid orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order is null)
            {
                return NotFound("order not found");
            }
            if (order.Status != OrderStatus.Pending)
            {
                return BadRequest("order has been already"+order.Status);
            }

            try
            {
                await _orderRepo.ChangeOrderStatusAsync(orderId, OrderStatus.Cancelled);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        
            return Ok();
        }
        // **************************************** End Of Hamed ****************************************
    }
}
