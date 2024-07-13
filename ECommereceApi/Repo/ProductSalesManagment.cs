using ECommereceApi.DTOs.Order;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class ProductSalesManagment : IProductSalesManagment
    {
        private readonly ECommerceContext _context;

        public ProductSalesManagment(ECommerceContext context)
        {
            this._context = context;
        }

        public IEnumerable<ProductDTO> GetProductsBestSellers()
        {
           IEnumerable<ProductDTO> products = _context.Products.Include(p => p.ProductImages)
                                                                    .Include(p => p.Category)
                                                                    .OrderByDescending(p => p.Score)
                                                                    .Select(p => new ProductDTO()
                                                                    {
                                                                        Name = p.Name,
                                                                        Discount = p.Discount,
                                                                        Amount = p.Amount,
                                                                        CategoryId = p.CategoryId,
                                                                        Description = p.Description,
                                                                        FinalPrice = p.FinalPrice,
                                                                        OriginalPrice = p.OriginalPrice,
                                                                        ProductId = p.ProductId,
                                                                        Score = p.Score,
                                                                        ProductImages = p.ProductImages.Select(image => new ProductImageDTO() { ImageId=image.ImageId}).ToList(),
                                                                        CategoryName = p.Category.Name
                                                                    });
        
            return products;
        
        }

        public IEnumerable<ProductDTO> GetProductsBestSellersPagination(int index=0, int pageSize=5)
        {
            return GetProductsBestSellers().Skip(index*pageSize).Take(pageSize).ToList();
        }

        public async Task<Status> UpdateOrderProductsScores(Guid orderId, List<ProductOrderStockDTO> productOrderStockDTOs)
        {
            return Status.Success;
            
        }

        public async Task<double> UpdateProductScore(int productId)
        {
            Product? product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
                throw new ArgumentException("Product Id is incorrect!");

            int amountSold = await _context.ProductOrders
                                            .Where(p => p.ProductId == productId)
                                            .SumAsync(p => p.Amount);

            double avgReview = await _context.Rates
                                                .Where(r => r.ProductId == productId)
                                                .Select(r => r.NumOfStars)
                                                .AverageAsync() ?? 0.0;

            double normalizedSalesCount = Math.Log(amountSold + 1);

            double normalizedAvgReviewScore = avgReview / 5.0;

            double score = 0.8 * normalizedSalesCount + 0.2 * normalizedAvgReviewScore;
            product.Score = score;
            await _context.SaveChangesAsync();
            return score;
        }


        public async Task<IEnumerable<(int productId, double score)>> UpdateProductsScores(IEnumerable<int> productIds)
        {
            var tasks = productIds.Select(async productId =>
            {
                double score = await UpdateProductScore(productId);
                return (productId, score);
            });

            var productsScores = await Task.WhenAll(tasks);
            return productsScores;
        }




    }
}