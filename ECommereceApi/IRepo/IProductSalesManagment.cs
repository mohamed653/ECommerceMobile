using ECommereceApi.DTOs.Order;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IProductSalesManagment
    {
        Task<Status> UpdateOrderProductsScores(Guid orderId, List<ProductOrderStockDTO> productOrderStockDTOs);
        decimal UpdateProductScore(int productId);

        IEnumerable<(int productId, decimal score)> UpdateProductsScores(IEnumerable<int> productsIds);

        
    }
}
