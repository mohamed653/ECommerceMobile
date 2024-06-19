using AutoMapper;
using ECommereceApi.Data;
using ECommereceApi.DTOs.Product;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class CartRepo : ICartRepo
    {
        private readonly ECommerceContext _db;
        private readonly IMapper _mapper;
        private readonly IProductRepo _productRepo;
        public CartRepo(ECommerceContext db, IMapper mapper, IProductRepo productRepo)
        {
            _db = db;
            _mapper = mapper;
            _productRepo = productRepo;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            User user = await _db.Users.Include(u => u.ProductCarts).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<CartProductsDTO> GetCartProductsAsync(User user)
        {

            var result = user.ProductCarts;
            if (result == null)
            {
                return null;
            }
            CartProductsDTO cartProducts = new CartProductsDTO();
            cartProducts.UserId = user.UserId;
            foreach (var item in result)
            {
                ProductDisplayInCartDTO productDisplayDTO = _mapper.Map<ProductDisplayInCartDTO>(await _productRepo.GetProductByIdAsync(item.ProductId));
                productDisplayDTO.Amount = item.ProductAmount;
                productDisplayDTO.AllAmount = _db.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId).Result.Amount;
                cartProducts.ProductsAmounts.Add(productDisplayDTO);
                cartProducts.numberOfUniqueProducts++;
                cartProducts.numberOfProducts += item.ProductAmount;
            }
            cartProducts.FinalPrice = cartProducts.ProductsAmounts.Sum(pa => pa.FinalPrice.Value * pa.Amount);
            return cartProducts;
        }
        public async Task DeleteCartItemsAsync(User user)
        {
            user.ProductCarts.Clear();
            _db.SaveChangesAsync();
        }
        public async Task AddProductToCartAsync(User user, ProductDisplayDTO product, int amount)
        {
            int maxProductAmount = product.Amount;
            int actualAmount = maxProductAmount >= amount ? amount : maxProductAmount;
            user.ProductCarts.Add(new ProductCart()
            {
                ProductId = product.ProductId,
                UserId = user.UserId,
                ProductAmount = actualAmount
            });
            _db.SaveChangesAsync();
        }
        public async Task ClearAndAddMultipleProducts(User user, List<int> productsIds, List<int> amounts)
        {
            DeleteCartItemsAsync(user);
            for(int i = 0; i < productsIds.Count; i++)
            {
                user.ProductCarts.Add(new ProductCart()
                {
                    ProductId = productsIds[i],
                    ProductAmount = amounts[i]
                });
            }
            _db.SaveChangesAsync();
        }
    }
}
