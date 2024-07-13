using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BestSellersController : ControllerBase
    {
        private readonly IProductSalesManagment _productRepo;

        public BestSellersController(IProductSalesManagment productRepo)
        {
            this._productRepo = productRepo;
        }


        [HttpGet("{index:int?}/{size:int?}")]
        public async Task<IActionResult> GetBestSellers(int index=0, int size=10)
        {
            if (index < 0)
                index = 0;

            if(size <= 0)
                size = 10;

            return Ok(_productRepo.GetProductsBestSellersPagination(index, size));
        }
    }
}
