using ECommereceApi.Models;
using ECommereceApi.IRepo;

namespace ECommereceApi.Repo
{
    public class ProductRepo: IProductRepo
    {
        private readonly ECommerceContext _context;

        public ProductRepo(ECommerceContext context)
        {
            _context = context;
        }

        public Status AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return Status.Success;
        }

        public Status DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return Status.Failed;
            _context.Products.Remove(product);
            _context.SaveChanges();

            return Status.Success;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.Find(id);
        }

        public Status UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
            return Status.Success;
        }
    }
}
