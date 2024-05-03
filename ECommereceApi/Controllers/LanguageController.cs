using ECommereceApi.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections;
using System.Globalization;
using System.Xml;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {

        private readonly ILanguageRepo _languageRepo;

        public LanguageController(ILanguageRepo languageRepo)
        {
            _languageRepo = languageRepo;
        }


        [HttpGet("GetAllLanguages")]
        public IActionResult GetAll(string culture = "ar-EG")
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            var result = _languageRepo.GetAll(culture);
            return Ok(result);
        }
        [HttpGet("GetByKey")]
        public IActionResult GetByKey(string key, string culture = "ar-EG")
        {
            if (key == null)
            {
                return BadRequest("Key is required");
            }

            var result = _languageRepo.GetValue(key, culture);
            if (result == null)
            {
                return NotFound($"This Key Input ({key}) Is not Found!");
            }
            return Ok(result);
        }

        [HttpPost("AddNewResource")]
        public IActionResult AddLocalizationResource(string key, string valueEN, string valueAR)
        {
            if (key == null)
            {
                return BadRequest("Key is required");
            }
            if (valueAR == null)
            {
                return BadRequest("Arabic Value is required");
            }
            if (valueEN == null)
            {
                return BadRequest("English Value is required");
            }

            _languageRepo.AddOrUpdateValue(key, valueAR, "ar-EG");
            _languageRepo.AddOrUpdateValue(key, valueEN, "en-US");

            return Ok($"The Key ({key}) has been added successfully!");
        }

        [HttpDelete("DeleteResource")]
        public IActionResult DeleteLocalizationResource(string key, string culture)
        {
            if (key == null)
            {
                return BadRequest("Key is required");
            }

            _languageRepo.DeleteValue(key, culture);

            return Ok($"The Key ({key}) has been deleted successfully!");
        }

        [HttpPut("UpdateResource")]
        public IActionResult UpdateLocalizationResource(string key, string valueEN, string valueAR)
        {
            if (key == null)
            {
                return BadRequest("Key is required");
            }
            if (valueAR == null)
            {
                return BadRequest("Arabic Value is required");
            }
            if (valueEN == null)
            {
                return BadRequest("English Value is required");
            }

            _languageRepo.AddOrUpdateValue(key, valueAR, "ar-EG");
            _languageRepo.AddOrUpdateValue(key, valueEN, "en-US");

            return Ok($"The Key ({key}) has been updated successfully!");
        }

    }
}
