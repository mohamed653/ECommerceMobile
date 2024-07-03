using ECommereceApi.DTOs.Order;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IProductSalesManagment
    {
        Task<Status> UpdateOrderProductsScores(Guid orderId, List<ProductOrderStockDTO> productOrderStockDTOs);
        Task<double> UpdateProductScore(int productId);

        Task<IEnumerable<(int productId, double score)>> UpdateProductsScores(IEnumerable<int> productsIds);

        IEnumerable<ProductDTO> GetProductsBestSellers();

        IEnumerable<ProductDTO> GetProductsBestSellersPagination(int index, int pageSize);

    }
}
