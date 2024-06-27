using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommereceApi.IRepo;

public interface IReviewRepo
{
    Task<IEnumerable<ReviewDTO>>? GetProductReviewsAsync(int productId);

    Task<ReviewDTO> GetUserReviewForProductAsync(int userId, int productId);

    Task<bool> TryAddReviewAsync(ReviewDTO reviewDto);

    Task<bool> TryUpdateProductAsync(ReviewDTO reviewDto);

    Task<bool> TryDeleteUserReviewAsync(int userId, int productId);

    Task<IEnumerable<ReviewDTO>>? GetProductReviewsPaginatrionAsync(int productId, int pageNo, int pageSize);

}
