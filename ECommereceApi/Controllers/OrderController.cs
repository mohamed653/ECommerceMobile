using Microsoft.AspNetCore.Mvc;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderRepo _orderRepo;
        public OrderController(IOrderRepo orderRepo)
        {
            _orderRepo = orderRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetOrder(int userId)
        {
            var result = await _orderRepo.GetOrderAsync(userId);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
