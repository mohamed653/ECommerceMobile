using ECommereceApi.Models;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IProductRepo
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(int id);
        Status AddProduct(Product product);
		Status UpdateProduct(Product product);
		Status DeleteProduct(int id);
    }
}
