
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommereceApi.Repo;

public class ReviewRepo : IReviewRepo
{
    private readonly ECommerceContext _context;
    public ReviewRepo(ECommerceContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<ReviewDTO>>? GetProductReviewsAsync(int productId)
    {
        try
        {
            IEnumerable<ReviewDTO>? productReviews = await _context.Rates.Where(r => r.ProductId == productId)
                                                                .Select(r => new ReviewDTO()
                                                                {
                                                                    ProductId = productId,
                                                                    CustomerId = r.CustomerId,
                                                                    Comment = r.Comment,
                                                                    NumOfStars = r.NumOfStars,
                                                                    RateDate = r.RateDate
                                                                }).ToListAsync();

            return productReviews;
        }
        catch
        {
            return null;
        }
        
    }

    public Task<IEnumerable<ReviewDTO>>? GetProductReviewsPaginatrionAsync(int productId, int pageNo, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<ReviewDTO?> GetUserReviewForProductAsync(int userId, int productId)
    {
        try
        {
            Rate? rate = await _context.Rates.FirstOrDefaultAsync(r => r.CustomerId == userId && r.ProductId == productId);

            if (rate is null)
                return null;

            ReviewDTO reviewDTO = new ReviewDTO()
            {
                CustomerId = rate.CustomerId,
                ProductId = rate.ProductId,
                Comment = rate.Comment,
                NumOfStars = rate.NumOfStars,
                RateDate = rate.RateDate
            };
            return reviewDTO;
        }
        catch
        {
            return null;
        }
        
    }

    public async Task<bool> TryAddReviewAsync(ReviewDTO reviewDto)
    {
        try
        {
            Rate rate = new Rate()
            {
                RateDate = reviewDto.RateDate,
                Comment = reviewDto.Comment,
                CustomerId = reviewDto.CustomerId,
                ProductId = reviewDto.ProductId,
                NumOfStars = reviewDto.NumOfStars,
            };

            await _context.Rates.AddAsync(rate);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TryDeleteUserReviewAsync(int userId, int productId)
    {
        try
        {
            ReviewDTO? reviewDTO = await GetUserReviewForProductAsync(userId, productId);
            if (reviewDTO is null)
                return false;

            Rate? rate = _context.Rates.FirstOrDefault(r => r.CustomerId == userId && r.ProductId == productId);

            _context.Rates.Remove(rate);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }   

    }

    public async Task<bool> TryUpdateProductAsync(ReviewDTO reviewDto)
    {
        try
        {

            Rate rate = await _context.Rates.FirstOrDefaultAsync(r => r.CustomerId == reviewDto.CustomerId && r.ProductId == reviewDto.ProductId);

            rate.Comment = reviewDto.Comment;
            rate.RateDate = reviewDto.RateDate;
            rate.NumOfStars = reviewDto.NumOfStars;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }


    }
}
