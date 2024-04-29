using ECommereceApi.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo productRepo;
        public ProductController(IProductRepo _productRepo)
        {
            productRepo = _productRepo;
        }
        [HttpGet]
        [Route("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            return Ok(productRepo.GetAllProducts());
        }


    }
}
