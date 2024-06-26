using AutoMapper;
using CloudinaryDotNet;
using ECommereceApi.DTOs.Order;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Enums;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using ECommereceApi.Services.classes;
using ECommereceApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
            AssignAllAmountPropertyToProductDisplayInCartDTOAsync(output.ProductsAmounts);
            var offers = await GetTodaysOffersAsync();
            output.ApplicableOffers = await GetApplicableOffersForCartAsync(cartProductsDTO, offers);
            return output;
        }
        public async Task AssignAllAmountPropertyToProductDisplayInCartDTOAsync(List<ProductDisplayInCartDTO> products)
        {
            foreach (var product in products)
                product.AllAmount = (await GetProductByIdAsync(product.ProductId)).Amount;
        }
        public async Task<OrderPreviewWithoutOffersDTO> AddOrderWithoutOfferAsync(CartProductsDTO cartProductsDTO, AddOrderWithoutOfferDTO addOrderWithoutOfferDTO)
        {
            var output = _mapper.Map<OrderPreviewWithoutOffersDTO>(addOrderWithoutOfferDTO);
            output.ProductsAmounts = cartProductsDTO.ProductsAmounts;
            var addedRecord = await _db.Orders.AddAsync(_mapper.Map<Order>(addOrderWithoutOfferDTO));
            await _db.SaveChangesAsync();
            foreach (var product in cartProductsDTO.ProductsAmounts)
            {
                var productOrder = new ProductOrder
                {
                    OrderId = addedRecord.Entity.OrderId,
                    ProductId = product.ProductId,
                    Amount = product.Amount
                };
                await _db.ProductOrders.AddAsync(productOrder);
            }
            await _db.SaveChangesAsync();
            AssignAdditionalValuestoOrderPreviewDTO(output);
            output.OrderId = addedRecord.Entity.OrderId;
            return output;
        }
        public async Task ChangeOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await GetOrderByIdAsync(orderId);
            order.Status = newStatus;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
        }
        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _db.Orders.FindAsync(orderId);
        }
        private void AssignAdditionalValuestoOrderPreviewDTO(OrderPreviewWithoutOffersDTO output)
        {
            output.FinalPrice = output.ProductsAmounts.Sum(p => p.FinalPrice.Value * p.Amount);
            output.numberOfProducts = output.ProductsAmounts.Sum(p => p.Amount);
            output.numberOfUniqueProducts = output.ProductsAmounts.Count;
            
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
            return await _db.Offers.Include(o => o.ProductOffers).Where(o => o.OfferDate.AddDays(o.Duration) >= DateOnly.FromDateTime(DateTime.Now) && o.OfferDate <= DateOnly.FromDateTime(DateTime.Now)).ToListAsync();
        }

        // Getting the Final total Order Price From the Cart
        //public async Task<decimal> GetFinalTotalPriceFromCart(int offerId,CartProductsDTO cartProductsDTO, )
        //{
        //    // 
        //}
    }
}
