using ECommereceApi.DTOs.Offer;
using ECommereceApi.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        #region Fields
        private readonly IOfferRepo offerRepo;

        #endregion

        #region Constructors
        public OffersController(IOfferRepo _offerRepo)
        {
            offerRepo = _offerRepo;
        }
        #endregion

        #region Get Methods
        /// <summary>
        /// Get all offers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllOffers()
        {
            try
            {
                return Ok(await offerRepo.GetOffers());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// returns all offers with their products
        /// </summary>
        /// <returns></returns>
        [HttpGet("withProducts")]

        public async Task<IActionResult> GetOffersWithProducts()
        {
            return Ok(await offerRepo.GetOffersWithProducts());
        }

        /// <summary>
        /// returns a specific offer by its ID with its products+ name and image (first default image)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfferById(int id)
        {
            try
            {
                var offer = await offerRepo.GetOfferById(id);
                if (offer == null) return NotFound();
                return Ok(offer);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        
        [HttpGet("byProductId/{productId}")]
        public async Task<IActionResult> GetOffersByProductId(int productId)
        {
            try
            {
                var offers = await offerRepo.GetOffersByProductId(productId);
                if (offers == null) return NotFound();
                return Ok(offers);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Post Methods

        /// <summary>
        /// Add a new offer  the date should be in this format "2024-5-1"
        /// </summary>
        /// <param name="offerDTO"></param>
        /// <returns>the offer Id</returns>
        [HttpPost]
        public async Task<IActionResult> AddOffer(OfferDTO offerDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                int createdOfferId = await offerRepo.AddOffer(offerDTO);
                // return the created offer ID 
                return CreatedAtAction(nameof(GetOfferById), new { id = createdOfferId }, createdOfferId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// assign product to a specific offer
        /// </summary>
        [HttpPost("{offerId}")]
        public async Task<IActionResult> AddProductsToOffer(int offerId, OffersDTOPost offerProductsDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var msg = await offerRepo.AddProductsToOffer(offerId, offerProductsDTO);
                return Ok(msg);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region Put Methods
        /// <summary>
        /// update an offer
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateOffer(int offerId,OfferDTO offerDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                await offerRepo.UpdateOffer(offerId, offerDTO);
                return NoContent();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// update products from an offer
        /// </summary>

        [HttpPut("{offerId}/{oldProductId}")]
        public async Task<IActionResult> UpdateProductsFromOffer(int offerId, int oldProductId ,OffersDTOPost offerProductsDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var status = await offerRepo.UpdateProductsFromOffer(offerId, oldProductId, offerProductsDTO);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(new {StatusMessage="Updated Successfully",StatusCode= Status.Success.ToString() });
        }

        #endregion

        #region Delete Methods

        /// <summary>
        /// delete an offer
        /// </summary>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [HttpDelete("{offerId}")]
        public async Task<IActionResult> DeleteOffer(int offerId)
        {
            try
            {
                await offerRepo.DeleteOffer(offerId);
                return NoContent();
            }
            catch (Exception e)
            {

                throw;
            }
        }
        /// <summary>
        ///  remove a product from an offer
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpDelete("{offerId}/products/{productId}")]
        public async Task<IActionResult> RemoveProductFromOffer(int offerId, int productId)
        {
            try
            {
                var offers = await offerRepo.RemoveProductFromOffer(offerId, productId);
                if(offers == null) return NotFound();
                return Ok(offers);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

    }
}
