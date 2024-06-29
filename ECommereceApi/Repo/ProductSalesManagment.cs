using ECommereceApi.DTOs.Order;

namespace ECommereceApi.Repo
{
    public class ProductSalesManagment : IProductSalesManagment
    {
        public async Task<Status> UpdateOrderProductsScores(Guid orderId, List<ProductOrderStockDTO> productOrderStockDTOs)
        {
            return Status.Success;
            
        }

        public decimal UpdateProductScore(int productId)
        {
            return 0m;
        }

        public IEnumerable<(int productId, decimal score)> UpdateProductsScores(IEnumerable<int> productsIds)
        {
            return productsIds.Select(productId => (productId, 0m));
        }
    }
}
