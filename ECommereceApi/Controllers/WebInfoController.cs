using ECommereceApi.DTOs.WebInfo;
using ECommereceApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebInfoController : ControllerBase
    {

        private readonly IWebInfoRepo _webInfoRepo;
        private readonly IFileCloudService _fileCloudService;
        public WebInfoController(IWebInfoRepo webInfoRepo)
        {
            _webInfoRepo = webInfoRepo;
        }
        /// <summary>
        /// get web info
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWebInfo()
        { 
            try
            {
                var webInfo = await _webInfoRepo.GetWebInfo();
                return Ok(webInfo);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// update web info
        /// </summary> 
        [HttpPut]
        public async Task<IActionResult> UpdateWebInfo(WebInfoDTO webInfoDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                await _webInfoRepo.UpdateWebInfo(webInfoDTO);
                return Ok();
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// add web info
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddWebInfo(WebInfoDTO webInfoDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                await _webInfoRepo.AddWebInfo(webInfoDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }

        }

    }
}
