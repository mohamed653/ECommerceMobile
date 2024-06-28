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
        private readonly IOfferRepo _offerRepo;
        public OrderRepo(IWebHostEnvironment env, ECommerceContext db, IMapper mapper, IFileCloudService fileCloudService, ICartRepo cartRepo, IOfferRepo offerRepo)
        {
            _db = db;
            _mapper = mapper;
            _env = env;
            _fileCloudService = fileCloudService;
            _cartRepo = cartRepo;
            _offerRepo = offerRepo;
        }
        public async Task<OrderPreviewDTO> GetOrderPreviewAsync(CartProductsDTO cartProductsDTO)
        {
            var output = _mapper.Map<OrderPreviewDTO>(cartProductsDTO);
            await AssignAllAmountPropertyToProductDisplayInCartDTOAsync(output.ProductsAmounts);
            var offers = await GetTodaysOffersAsync();
            output.ApplicableOffers = await GetApplicableOffersForCartAsync(cartProductsDTO, offers);
            return output;
        }
        public async Task AssignAllAmountPropertyToProductDisplayInCartDTOAsync(List<ProductDisplayInCartDTO> products)
        {
            foreach (var product in products)
                product.AllAmount = (await GetProductByIdAsync(product.ProductId)).Amount;
        }
        //public async Task<OrderPreviewWithoutOffersDTO> AddOrderWithoutOfferAsync(CartProductsDTO cartProductsDTO, AddOrderOfferDTO addOrderOfferDTO, double finalPrice)
        //{
        //    var output = _mapper.Map<OrderPreviewWithoutOffersDTO>(addOrderOfferDTO);
        //    output.ProductsAmounts = cartProductsDTO.ProductsAmounts;
        //    output.FinalPrice = 
        //    var addedRecord = await _db.Orders.AddAsync(_mapper.Map<Order>(addOrderOfferDTO));
        //    await _db.SaveChangesAsync();
        //    foreach (var product in cartProductsDTO.ProductsAmounts)
        //    {
        //        var productOrder = new ProductOrder
        //        {
        //            OrderId = addedRecord.Entity.OrderId,
        //            ProductId = product.ProductId,
        //            Amount = product.Amount
        //        };
        //        await _db.ProductOrders.AddAsync(productOrder);
        //    }
        //    await _db.SaveChangesAsync();
        //    AssignAdditionalValuestoOrderPreviewDTO(output);
        //    output.OrderId = addedRecord.Entity.OrderId;
        //    return output;
        //}
        // ** added By Hamed
        public async Task<OrderPreviewWithoutOffersDTO> AddOrderAsync(CartProductsDTO cartProductsDTO, AddOrderOfferDTO addOrderOfferDTO,double finalPrice)
        {
            var output = _mapper.Map<OrderPreviewWithoutOffersDTO>(addOrderOfferDTO);
            output.ProductsAmounts = cartProductsDTO.ProductsAmounts;
            output.FinalPrice = finalPrice;

            var _order = _mapper.Map<Order>(addOrderOfferDTO);
            _order.TotalPrice = finalPrice;
            _order.Status = OrderStatus.Pending;
            _order.TotalAmount = cartProductsDTO.numberOfProducts;

            var addedRecord = await _db.Orders.AddAsync(_order);
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
        public async Task<Product?> GetProductByIdAsync(int productId)
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


        // **************************************** Hamed ****************************************
        #region Hamed


        public async Task<PagedResult<OrderDisplayDTO>> GetAllOrdersPaginatedAsync(int page, int pageSize)
        {
            var orders = await _db.Orders.ToListAsync();
            return RenderPagination(page, pageSize, orders);
        }

        public async Task<PagedResult<OrderDisplayDTO>> GetOrdersByStatusPaginatedAsync(OrderStatus status, int page, int pageSize)
        {
            var orders = await _db.Orders.Where(o=>o.Status== status).ToListAsync();
            return RenderPagination(page, pageSize, orders);
        }
        public async Task<PagedResult<OrderDisplayDTO>> GetUserOrdersPaginatedAsync(int userId, int page, int pageSize)
        {
            var orders = await _db.Orders.Where(o => o.UserId == userId).ToListAsync();
            return RenderPagination(page, pageSize, orders);
        }
        public PagedResult<OrderDisplayDTO> RenderPagination(int page, int pageSize, List<Order> inputOrders)
        {
            PagedResult<OrderDisplayDTO> result = new PagedResult<OrderDisplayDTO>();
            int totalCount = inputOrders.Count;
            result.TotalItems = totalCount;
            result.TotalPages = totalCount / pageSize;
            if (totalCount % pageSize > 0)
                result.TotalPages++;
            result.PageSize = pageSize;
            result.PageNumber = page;
            result.HasPrevious = page != 1;
            result.HasNext = page != result.TotalPages;

            var orders = inputOrders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            result.Items = _mapper.Map<List<OrderDisplayDTO>>(orders);

            return result;
        }
        // *** Price
        public async Task<Tuple<int, int>> GetFinalOfferPriceAsync(int offerId, int userId)
        {
            var offer = await GetOfferByIdAsync(offerId);
            var cartProducts = await GetUserCartProductsAsync(userId);
            var cartFinalPrice = cartProducts.FinalPrice;
            

            await ValidateOfferAndCartProductsAsync(offer, cartProducts);

            var offerCalculationResult = await CalculateTotalItemsPriceInOfferAsync(offer, cartProducts);
            var totalItemsPriceInOffer = offerCalculationResult.Item1;
            var updatedCartProducts = offerCalculationResult.Item2;

            var totalNonOfferPrice = await CalculateTotalNonOfferPriceAsync(updatedCartProducts);

            var offerFinalPrice = totalItemsPriceInOffer + totalNonOfferPrice;

            return new Tuple<int, int>((int)Math.Ceiling(cartFinalPrice),(int)Math.Ceiling(offerFinalPrice));
        }


        private async Task<Offer> GetOfferByIdAsync(int offerId)
        {
            var todayOffers = await GetTodaysOffersAsync();
            var offer = todayOffers.FirstOrDefault(x => x.OfferId == offerId);

            if (offer == null)
            {
                throw new ArgumentException("Offer not found");
            }

            return offer;
        }

        private async Task<CartProductsDTO> GetUserCartProductsAsync(int userId)
        {
            var user = await _cartRepo.GetUserByIdAsync(userId);
            return await _cartRepo.GetCartProductsAsync(user);
        }

        private async Task ValidateOfferAndCartProductsAsync(Offer offer, CartProductsDTO cartProducts)
        {
            bool isOfferApplicable = await IsOfferApplicableAsync(offer, cartProducts);
            bool areItemsAvailable = await IsAllCartItemsAvailableAsync(cartProducts);

            if (!isOfferApplicable || !areItemsAvailable)
            {
                throw new InvalidOperationException("Offer is not applicable or items are not available");
            }
        }

        private async Task<double> CalculateTotalNonOfferPriceAsync(CartProductsDTO cartProducts)
        {
            return cartProducts.ProductsAmounts.Sum(x => x.FinalPrice.Value * x.Amount);
        }

        private async Task<Tuple<double, CartProductsDTO>> CalculateTotalItemsPriceInOfferAsync(Offer offer, CartProductsDTO cartProducts)
        {
            double totalItemsPriceInOffer = 0;

            foreach (var productOffer in offer.ProductOffers)
            {
                totalItemsPriceInOffer += CalculateDiscountedPrice(productOffer, cartProducts);
            }

            totalItemsPriceInOffer -= (double)offer.PackageDiscount;

            UpdateCartProductsForOffer(cartProducts, offer);

            return new Tuple<double, CartProductsDTO>(totalItemsPriceInOffer, cartProducts);
        }

        private double CalculateDiscountedPrice(ProductOffer productOffer, CartProductsDTO cartProducts)
        {
            var product = cartProducts.ProductsAmounts.FirstOrDefault(x => x.ProductId == productOffer.ProductId);
            if (product != null)
            {
                double discountRate = (productOffer.Discount ?? 0) / 100.0;
                double discountedPricePerUnit = (product.FinalPrice ?? 0) * (1 - discountRate);
                return discountedPricePerUnit * productOffer.ProductAmount;
            }
            return 0;
        }

        private void UpdateCartProductsForOffer(CartProductsDTO cartProducts, Offer offer)
        {
            foreach (var productOffer in offer.ProductOffers)
            {
                var product = cartProducts.ProductsAmounts.FirstOrDefault(x => x.ProductId == productOffer.ProductId);
                if (product != null)
                {
                    product.Amount -= productOffer.ProductAmount;
                    if (product.Amount <= 0)
                    {
                        cartProducts.ProductsAmounts.Remove(product);
                    }
                }
            }
        }

        // *** end of Price

        public async Task<Guid> ConfirmOrder(AddOrderOfferDTO addOrderOfferDTO)
        {
            OrderPreviewWithoutOffersDTO orderPreview =new ();
            // Get user by id
            var user = await _cartRepo.GetUserByIdAsync(addOrderOfferDTO.UserId);
            // Get Cart By User Id
            var _cartProductsDTO = await _cartRepo.GetCartProductsAsync(user);

            // If No Offer Assigned or Not Applicable no more <==
            if (addOrderOfferDTO.OfferId == null|| addOrderOfferDTO.OfferId == 0 )
            {
                addOrderOfferDTO.OfferId = null;
                // Get Total Price of the cart
               var totalCartPrice = _cartProductsDTO.FinalPrice;
               orderPreview = await  AddOrderAsync(_cartProductsDTO, addOrderOfferDTO, totalCartPrice);
            }
            // If Offer Assigned
            else
            {
                // Get the final price of the offer
                var finalPriceAferOffer = GetFinalOfferPriceAsync(addOrderOfferDTO.OfferId.Value, addOrderOfferDTO.UserId).Result.Item2;

                orderPreview = await AddOrderAsync(_cartProductsDTO, addOrderOfferDTO, finalPriceAferOffer);

            }
            // Notify the Admins ***************** using Notification Service *****************


            // Delete the user cart
            await _cartRepo.DeleteCartItemsAsync(user);

            // return order
            return orderPreview.OrderId;

        }


        #endregion 

        // **************************************** End Of Hamed ****************************************
    }
}
