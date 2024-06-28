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
            return await _db.Users.Include(u => u.ProductCarts).FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<CartProductsDTO?> GetCartProductsAsync(User user)
        {
            return await MapProductsInCartIntoCartProductsDTO(user.ProductCarts, new CartProductsDTO());
        }

        public async Task DeleteCartItemsAsync(User user)
        {
            user.ProductCarts.Clear();
            await MySaveChangesAsync();
        }
        public async Task AddProductsToCartAsync(User user, List<int> productsIds, List<int> amounts)
        {
            AddMultipleProductsToCart(user.ProductCarts, productsIds, amounts);
            await MySaveChangesAsync();
        }
        public async Task AddProductToCartAsync(User user, ProductDisplayDTO product, int amount)
        {
            user.ProductCarts.Add(new ProductCart()
            {
                ProductId = product.ProductId,
                UserId = user.UserId,
                ProductAmount = amount
            });
            await MySaveChangesAsync();
        }
        public async Task MySaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public bool IsUserHaveProductsInCart(User user)
        {
            return user.ProductCarts.Count != 0;
        }
        public void AddMultipleProductsToCart(ICollection<ProductCart> productsInCart, List<int> productsIds, List<int> amounts)
        {
            for (int i = 0; i < productsIds.Count; i++)
            {
                productsInCart.Add(new ProductCart()
                {
                    ProductId = productsIds[i],
                    ProductAmount = amounts[i]
                });
            }
        }
        public async Task<CartProductsDTO> MapProductsInCartIntoCartProductsDTO(ICollection<ProductCart> products, CartProductsDTO cartProducts)
        {
            foreach (var item in products)
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
    }
}
