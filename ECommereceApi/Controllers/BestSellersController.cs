using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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


        /// <summary>
        /// Gets a paginated list of best-selling products.
        /// </summary>
        /// <param name="index">The starting index of the pagination. Default is 0.</param>
        /// <param name="size">The number of items per page. Default is 10.</param>
        /// <returns>A list of best-selling products.</returns>
        [HttpGet("{index:int?}/{size:int?}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
