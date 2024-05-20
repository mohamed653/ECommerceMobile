using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly IOfferRepo offerRepo;

        public OffersController(IOfferRepo _offerRepo)
        {
            offerRepo = _offerRepo;
        }

        /// <summary>
        /// Get all offers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOffers()
        {
            return Ok(await offerRepo.GetOffers());
        }


        /// <summary>
        /// returns all offers with their products
        /// </summary>
        /// <returns></returns>
        [HttpGet("/withProducts")]

        public async Task<IActionResult> GetOffersWithProducts()
        {
            return Ok(await offerRepo.GetOffersWithProducts());
        }

        /// <summary>
        /// returns a specific offer by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfferById(int id)
        {
            var offer = await offerRepo.GetOfferById(id);
            if (offer == null) return NotFound();
            return Ok(offer);
        }

        /// <summary>
        /// Add a new offer
        /// </summary>
        /// <param name="offerDTO"></param>
        /// <returns>the offer Id</returns>
        [HttpPost]
        public async Task<IActionResult> AddOffer(OfferDTO offerDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int createdOfferId= await offerRepo.AddOffer(offerDTO);
            //return the created offer ID 
            return CreatedAtAction(nameof(GetOfferById), new { id = createdOfferId }, createdOfferId);

        }

        /// <summary>
        /// assign products to a specific offer
        /// </summary>
        [HttpPost("{offerId}/products")]
        public async Task<IActionResult> AddProductsToOffer(int offerId, List<OfferProductsDTO> offerProductsDTOs, decimal? PackageDiscount)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await offerRepo.AddProductsToOffer( offerId, offerProductsDTOs,PackageDiscount);
            return Created();
        }

        /// <summary>
        /// update an offer
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateOffer(OffersDTOUI offersDTOUI)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await offerRepo.UpdateOffer(offersDTOUI);
            return NoContent();
        }

        /// <summary>
        /// delete an offer
        /// </summary>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [HttpDelete("{offerId}")]
        public async Task<IActionResult> DeleteOffer(int offerId)
        {
            await offerRepo.DeleteOffer(offerId);
            return NoContent();
        }




    }
}
