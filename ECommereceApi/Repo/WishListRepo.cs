using AutoMapper;
using ECommereceApi.DTOs.Product;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ECommereceApi.Repo
{
    public class WishListRepo: IWishListRepo
    {
        private readonly ECommerceContext _context;
        private readonly IProductRepo _productRepo;
        private readonly IMapper _mapper;
        public WishListRepo(ECommerceContext context, IProductRepo productRepo,IMapper mapper)
        {
            _context = context;
            _productRepo = productRepo;
            _mapper = mapper;
        }
        public async Task<WishList> AddWishList(WishListDTO wishListDTO)
        {
            var product = await _context.Products.FindAsync(wishListDTO.ProductId);
            if (product == null)
            {
                return null;
            }
            var user = await _context.Users.FindAsync(wishListDTO.UserId);
            if (user == null)
            {
                return null;
            }
            var wishList = new WishList
            {
                UserId = wishListDTO.UserId,
                User = user,
                Product = product,
                ProductId = wishListDTO.ProductId
            };
            await _context.WishLists.AddAsync(wishList);
            await _context.SaveChangesAsync();
            return wishList;
        }

        public async Task<List<WishList>> DeleteWishListItem(int userId, int productId)
        {
            var wishList = await _context.WishLists.Where(x => x.UserId == userId && x.ProductId == productId).ToListAsync();
            if (wishList == null)
            {
                return wishList;
            }
            _context.WishLists.RemoveRange(wishList);
            await _context.SaveChangesAsync();
            return wishList;
        }

        public async Task<Status> DeleteWishList(int id)
        {
            var wishList = await _context.WishLists.FindAsync(id);
            if (wishList == null)
            {
                return Status.NotFound;
            }
            _context.WishLists.Remove(wishList);
            await _context.SaveChangesAsync();
            return Status.Deleted;
        }

        public async Task<List<WishList>> GetWishListByUserId(int userId)
        {
            return await _context.WishLists.Include(x=>x.Product).Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<List<ProductDisplayDTO>> GetWishListProducts(int userId)
        {
            var wishLists = GetWishListByUserId(userId);
            List<ProductDisplayDTO> products = new List<ProductDisplayDTO>();

            foreach(var wishList in wishLists.Result)
            {
                var product = await _productRepo.GetProductByIdAsync(wishList.ProductId);
                if(product.ProductImages.Count > 1)
                    product.ProductImages = product.ProductImages.Take(1).ToList();
                products.Add(product);
            }

            return products;
        }

        public async Task<int> GetWishListCount(int userId)
        {
            return await _context.WishLists.Where(x => x.UserId == userId).CountAsync();
        }

        public async Task<List<ProductDisplayDTO>> GetTopWishlistedProducts()
        {
            var products = await _context.WishLists.GroupBy(x => x.ProductId)
                .Select(x => new { ProductId = x.Key, Count = x.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();
            List<ProductDisplayDTO> topProducts = new List<ProductDisplayDTO>();
            foreach (var product in products)
            {
                var p = await _productRepo.GetProductByIdAsync(product.ProductId);
                topProducts.Add(p);
            }
            return topProducts;
        }
    }
}
