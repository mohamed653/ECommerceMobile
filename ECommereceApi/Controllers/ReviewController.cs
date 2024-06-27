using ECommereceApi.Models;
using ECommereceApi.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            IEnumerable<ReviewDTO>? reviews = await _reviewRepo.GetProductReviewsAsync(productId);
            if (reviews is null)
                return Content("no reviews for this product yet!");

            return Ok(reviews);
        }

        [HttpGet("{productId:int}/{customerId:int}")]
        public async Task<IActionResult> GetCustomerReview(int productId ,int customerId)
        {
            ReviewDTO reviewDto =await _reviewRepo.GetUserReviewForProductAsync(customerId, productId);
            if (reviewDto is null)
                return BadRequest("User Id or product Id is incorrect!");

            return Ok(reviewDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductReview(ReviewDTO reviewDTO)
        {
            if (reviewDTO is null)
                return BadRequest("No Review!!");

          
            if (! ModelState.IsValid)
            {
                IEnumerable<string> errors = ModelState.Values.SelectMany(v => v.Errors)
                                                              .Select(error => error.ErrorMessage);
                
                return BadRequest(errors);
            }

            ReviewDTO reviewDto = await _reviewRepo.GetUserReviewForProductAsync(reviewDTO.CustomerId, reviewDTO.ProductId);
            if (reviewDto is not null)
                return BadRequest("customer can't have multiple reviews for the same product!!");


            bool reviewAdded = await _reviewRepo.TryAddReviewAsync(reviewDTO);
            if (!reviewAdded)
                return BadRequest("Review not added!!");

            return Created("/api/review",reviewDTO);
        }


        [HttpPut()]
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


        [HttpDelete("{productId:int}/{customerId:int}")]
        public async Task<IActionResult> DeleteUserReview(int productId, int customerId)
        {
            bool isDeleted =await _reviewRepo.TryDeleteUserReviewAsync(customerId, productId);
            if (isDeleted)
                return Ok();

            return BadRequest("product Id or customer Id is incorrect!");
        }


    }
}
