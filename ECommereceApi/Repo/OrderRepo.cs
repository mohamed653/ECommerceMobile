using AutoMapper;
using CloudinaryDotNet;
using ECommereceApi.DTOs.Order;
using ECommereceApi.DTOs.Product;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using ECommereceApi.Services.classes;
using ECommereceApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{

    public class OrderRepo : IOrderRepo
    {
        private readonly ECommerceContext _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IFileCloudService _fileCloudService;
        private readonly ICartRepo _cartRepo;
        public OrderRepo(IWebHostEnvironment env, ECommerceContext db, IMapper mapper, IFileCloudService fileCloudService, ICartRepo cartRepo)
        {
            _db = db;
            _mapper = mapper;
            _env = env;
            _fileCloudService = fileCloudService;
            _cartRepo = cartRepo;
        }
        public async Task<OrderPreviewDTO> GetOrderPreviewAsync(CartProductsDTO cartProductsDTO)
        {
            var output = _mapper.Map<OrderPreviewDTO>(cartProductsDTO);
            var offers = await GetTodaysOffersAsync();
            output.ApplicableOffers = await GetApplicableOffersForCartAsync(cartProductsDTO, offers);
            return output;
        }
        public async Task<List<OfferDisplayDTO>> GetApplicableOffersForCartAsync(CartProductsDTO cartProductsDTO, List<Offer> offers)
        {
            List<OfferDisplayDTO> applicableOffers = new List<OfferDisplayDTO>();
            foreach (var offer in offers)
            {
                if (await IsOfferApplicableAsync(offer, cartProductsDTO))
                {
                    applicableOffers.Add(_mapper.Map<OfferDisplayDTO>(offer));
                }
            }
            return applicableOffers;
        }
        public async Task<bool> IsAllCartItemsAvailableAsync(CartProductsDTO cartProducts)
        {
            var results = cartProducts.ProductsAmounts.Select(p => IsProductExistWithAmounts(p)).ToList();
            return results.All(r => r);
        }
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _db.Products.SingleOrDefaultAsync(p => p.ProductId == productId);
        }
        public bool IsProductExistWithAmounts(ProductDisplayInCartDTO product)
        {
            return _db.Products.Any(p => p.ProductId == product.ProductId && p.Amount >= product.Amount);
        }
        public async Task<bool> IsOfferApplicableAsync(Offer offer, CartProductsDTO cartProductsDTO)
        {
            bool isApplicable = false;
            foreach (var productOffer in offer.ProductOffers)
            {
                if (cartProductsDTO.ProductsAmounts.Any(p => p.ProductId == productOffer.ProductId && p.Amount >= productOffer.ProductAmount))
                {
                    isApplicable = true;
                }
                else
                {
                    isApplicable = false;
                }
            }
            return isApplicable;
        }
        public async Task<List<Offer>> GetTodaysOffersAsync()
        {
            return await _db.Offers.Include(o => o.ProductOffers).Where(o => o.OfferDate.AddDays(o.Duration.Value) >= DateOnly.FromDateTime(DateTime.Now) && o.OfferDate <= DateOnly.FromDateTime(DateTime.Now)).ToListAsync();
        }
    }
}
