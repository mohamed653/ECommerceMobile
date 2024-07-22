using ECommereceApi.Models;
using ECommereceApi.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepo _reviewRepo;
        public ReviewController(IReviewRepo reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        /// <summary>
        /// Gets all reviews for a specific product.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>List of reviews for the product.</returns>
        [HttpGet("{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            IEnumerable<ReviewDTO>? reviews = await _reviewRepo.GetProductReviewsAsync(productId);
            if (reviews is null)
                return Content("No reviews for this product yet!");

            return Ok(reviews);
        }

        /// <summary>
        /// Gets a specific review by a customer for a product.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="customerId">The ID of the customer.</param>
        /// <returns>The review by the customer for the product.</returns>
        [HttpGet("{productId:int}/{customerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCustomerReview(int productId, int customerId)
        {
            ReviewDTO reviewDto = await _reviewRepo.GetUserReviewForProductAsync(customerId, productId);
            if (reviewDto is null)
                return BadRequest("User ID or product ID is incorrect!");

            return Ok(reviewDto);
        }

        /// <summary>
        /// Adds a new review for a product.
        /// </summary>
        /// <param name="reviewDTO">The review details.</param>
        /// <returns>Created if successful, otherwise BadRequest.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> AddProductReview(ReviewDTO reviewDTO)
        {
            if (reviewDTO is null)
                return BadRequest("No Review!");

            if (!ModelState.IsValid)
            {
                IEnumerable<string> errors = ModelState.Values.SelectMany(v => v.Errors)
                                                              .Select(error => error.ErrorMessage);

                return BadRequest(errors);
            }

            ReviewDTO reviewDto = await _reviewRepo.GetUserReviewForProductAsync(reviewDTO.CustomerId, reviewDTO.ProductId);
            if (reviewDto is not null)
                return BadRequest("Customer can't have multiple reviews for the same product!");

            bool reviewAdded = await _reviewRepo.TryAddReviewAsync(reviewDTO);
            if (!reviewAdded)
                return BadRequest("Review not added!");

            return Created("/api/review", reviewDTO);
        }

        /// <summary>
        /// Updates an existing review for a product.
        /// </summary>
        /// <param name="review">The review details to update.</param>
        /// <returns>OK if successful, otherwise BadRequest.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(ReviewDTO review)
        {
            if (review is null)
                return BadRequest("Review can't be null!");

            ReviewDTO? existingReview = await _reviewRepo.GetUserReviewForProductAsync(review.CustomerId, review.ProductId);
            if (existingReview is null)
                return BadRequest("Customer has no review on this product!");

            if (!ModelState.IsValid)
            {
                IEnumerable<string> errors = ModelState.Values.SelectMany(v => v.Errors)
                                                    .Select(error => error.ErrorMessage);

                return BadRequest(errors);
            }

            bool isUpdated = await _reviewRepo.TryUpdateProductAsync(review);
            if (!isUpdated)
                return BadRequest("Review isn't updated!");

            return Ok();
        }

        /// <summary>
        /// Deletes a review by a customer for a product.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="customerId">The ID of the customer.</param>
        /// <returns>OK if successful, otherwise BadRequest.</returns>
        [HttpDelete("{productId:int}/{customerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> DeleteUserReview(int productId, int customerId)
        {
            bool isDeleted = await _reviewRepo.TryDeleteUserReviewAsync(customerId, productId);
            if (isDeleted)
                return Ok();

            return BadRequest("Product ID or customer ID is incorrect!");
        }
    }
}
