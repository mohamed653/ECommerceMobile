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
using Serilog;

namespace ECommereceApi.Repo
{

    public class OrderRepo : IOrderRepo
    {
        #region Fields
        private readonly ECommerceContext _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IFileCloudService _fileCloudService;
        private readonly ICartRepo _cartRepo;
        private readonly IOfferRepo _offerRepo;
        private readonly IProductRepo _productRepo;
        private readonly IProductSalesManagment _productSalesManagment;
        #endregion

        #region Ctor
        public OrderRepo(IWebHostEnvironment env, ECommerceContext db, IMapper mapper, IFileCloudService fileCloudService, ICartRepo cartRepo, IOfferRepo offerRepo, IProductRepo productRepo, IProductSalesManagment productSalesManagment)
        {
            _db = db;
            _mapper = mapper;
            _env = env;
            _fileCloudService = fileCloudService;
            _cartRepo = cartRepo;
            _offerRepo = offerRepo;
            _productRepo = productRepo;
            _productSalesManagment = productSalesManagment;
        }
        #endregion

        #region Get Functions
        public async Task<OrderPreviewDTO> GetOrderPreviewAsync(CartProductsDTO cartProductsDTO)
        {
            var output = _mapper.Map<OrderPreviewDTO>(cartProductsDTO);
            await AssignAllAmountPropertyToProductDisplayInCartDTOAsync(output.ProductsAmounts);
            var offers = await GetTodaysOffersAsync();
            output.ApplicableOffers = await GetApplicableOffersForCartAsync(cartProductsDTO, offers);
            return output;
        }
        public async Task<OrderDisplayDTO> GetOrderByIdAsync(Guid orderId)
        {
            return _mapper.Map<OrderDisplayDTO>( await _db.Orders.Include(u=>u.User).Include(o=>o.ProductOrders).FirstOrDefaultAsync(x=>x.OrderId ==orderId));
        }
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _db.Products.SingleOrDefaultAsync(p => p.ProductId == productId);
        }
        public async Task<List<Offer>> GetTodaysOffersAsync()
        {
            return await _db.Offers.Include(o => o.ProductOffers).Where(o => o.OfferDate.AddDays(o.Duration) >= DateOnly.FromDateTime(DateTime.Now) && o.OfferDate <= DateOnly.FromDateTime(DateTime.Now)).ToListAsync();
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
        #endregion

        #region Exec Functions
        private void AssignAdditionalValuestoOrderPreviewDTO(OrderPreviewWithoutOffersDTO output)
        {
            output.FinalPrice = output.ProductsAmounts.Sum(p => p.FinalPrice.Value * p.Amount);
            output.numberOfProducts = output.ProductsAmounts.Sum(p => p.Amount);
            output.numberOfUniqueProducts = output.ProductsAmounts.Count;
        }

        public async Task AssignAllAmountPropertyToProductDisplayInCartDTOAsync(List<ProductDisplayInCartDTO> products)
        {
            foreach (var product in products)
                product.AllAmount = (await GetProductByIdAsync(product.ProductId)).Amount;
        }

        #endregion

        #region Checks
        public async Task<bool> IsAllCartItemsAvailableAsync(CartProductsDTO cartProducts)
        {
            var results = cartProducts.ProductsAmounts.Select(p => IsProductExistWithAmounts(p)).ToList();
            return results.All(r => r);
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

        #endregion

        // **************************************** Hamed ****************************************
        #region Order Paginations


        public async Task<PagedResult<OrderDisplayDTO>> GetAllOrdersPaginatedAsync(int page, int pageSize)
        {
            var _orders = _mapper.Map<List<OrderDisplayDTO>>(await _db.Orders.Include(x=>x.User)
                                                                            .Include(x => x.ProductOrders)
                                                                            .ThenInclude(x => x.Product)
                                                                            .ThenInclude(x => x.ProductImages).ToListAsync());
            return RenderPagination(page, pageSize, _orders);
        }

        public async Task<PagedResult<OrderDisplayDTO>> GetOrdersByStatusPaginatedAsync(OrderStatus status, int page, int pageSize)
        {
            var _orders = _mapper.Map<List<OrderDisplayDTO>>(await _db.Orders.Where(o=>o.Status == status)
                                                                            .Include(x => x.ProductOrders).ThenInclude(x => x.Product)
                                                                            .ThenInclude(x => x.ProductImages)
                                                                            .ToListAsync());
            return RenderPagination(page, pageSize, _orders);
        }
        public async Task<PagedResult<OrderDisplayDTO>> GetUserOrdersPaginatedAsync(int userId, int page, int pageSize)
        {
            var _orders = _mapper.Map<List<OrderDisplayDTO>>( await _db.Orders.Where(o=>o.UserId== userId)
                                                                                .Include(x=>x.ProductOrders).ThenInclude(x=>x.Product)
                                                                                .ThenInclude(x=>x.ProductImages)
                                                                                .ToListAsync());
            return RenderPagination(page, pageSize, _orders);
        }
        public async Task<PagedResult<OrderDisplayDTO>> GetUserOrdersByStatusPaginatedAsync(int userId, OrderStatus orderStatus, int page, int pageSize)
        {
            var _orders = _mapper.Map<List<OrderDisplayDTO>>(await _db.Orders.Where(o => o.UserId == userId&&o.Status == orderStatus)
                                                                             .Include(x => x.ProductOrders)
                                                                             .ThenInclude(x => x.Product)
                                                                             .ThenInclude(x => x.ProductImages)
                                                                             .ToListAsync());
            return RenderPagination(page, pageSize, _orders);
        }
        public PagedResult<OrderDisplayDTO> RenderPagination(int page, int pageSize, List<OrderDisplayDTO> inputOrders)
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
            result.Items = orders;

            return result;
        }
        #endregion
        
        #region Order Offer Price Calculation
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

        #endregion

        #region Order Confirmation
        public async Task<OrderPreviewWithoutOffersDTO> AddOrderAsync(CartProductsDTO cartProductsDTO, AddOrderOfferDTO addOrderOfferDTO, double finalPrice)
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

        #region Order Acceptance, Shipment and Delivery

        public async Task ChangeOrderStatusAsync(Guid orderId, OrderStatus newStatus, int arrivalInDays = 0)
        {
            var order = await _db.Orders.Include(x=>x.ProductOrders).FirstOrDefaultAsync(x => x.OrderId == orderId);
            List<ProductOrderStockDTO> productOrderStockDTOs = order.ProductOrders.Select(x => new ProductOrderStockDTO
            {
                ProductId = x.ProductId,
                Amount = x.Amount
            }).ToList();

            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            if (order.Status == newStatus)
            {
                throw new InvalidOperationException($"Order status is already {newStatus}");
            }

            if (order.Status != OrderStatus.Cancelled && order.Status > newStatus)
            {
                throw new InvalidOperationException($"Order status can't be reversed to {newStatus}");
            }

            try
            {
                switch (newStatus)
                {
                    case OrderStatus.Accepted:
                        await _productRepo.SubtractProductAmountFromStock(productOrderStockDTOs);
                        break;

                    case OrderStatus.Shipped:
                        order.ShippingDate = DateOnly.FromDateTime(DateTime.Now);
                        order.ArrivalInDays = arrivalInDays;
                        // Call Notification Service to Notify the User
                        break;

                    case OrderStatus.Delivered:
                        await _productSalesManagment.UpdateOrderProductsScores(orderId, productOrderStockDTOs);
                        // Call Notification Service to Notify the Admin
                        break;

                    case OrderStatus.Cancelled:
                        // Call Notification Service to Notify the User
                        break;
                }

                order.Status = newStatus;

                _db.Attach(_mapper.Map<Order>(order)); // Ensure the entity is attached to the context
                _db.Entry(order).State = EntityState.Modified; // Mark the entity as modified
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"OrderRepo | ChangeOrderStatusAsync() OrderId:{orderId} ErrorMessage:{ex.Message}");
                throw;
            }
        }



        #endregion

        #region Order Statistics

        private async Task<int> GetTotalOrdersCountAsync()
        {
            return await _db.Orders.CountAsync();
        }
        private async Task<int> GetTotalOrdersCountByStatusAsync(OrderStatus status)
        {
            return await _db.Orders.CountAsync(x => x.Status == status);
        }
        private async Task<int> GetTotalOrdersCountByStatusAsync(OrderStatus status,int userId)
        {
            return await _db.Orders.CountAsync(x => x.Status == status && x.UserId == userId);
        }
        public async Task<OrderStatsDTO> GetUserOrderStats(int userId)
        {
            var totalOrders = await GetTotalOrdersCountByStatusAsync(OrderStatus.Pending, userId);
            var totalPendingOrders = await GetTotalOrdersCountByStatusAsync(OrderStatus.Pending, userId);
            var totalAcceptedOrders = await GetTotalOrdersCountByStatusAsync(OrderStatus.Accepted, userId);
            var totalShippedOrders = await GetTotalOrdersCountByStatusAsync(OrderStatus.Shipped, userId);
            var totalDeliveredOrders = await GetTotalOrdersCountByStatusAsync(OrderStatus.Delivered, userId);
            var totalCancelledOrders = await GetTotalOrdersCountByStatusAsync(OrderStatus.Cancelled, userId);

            return new OrderStatsDTO
            {
                TotalOrders = totalOrders,
                TotalPendingOrders = totalPendingOrders,
                TotalAcceptedOrders = totalAcceptedOrders,
                TotalShippedOrders = totalShippedOrders,
                TotalDeliveredOrders = totalDeliveredOrders,
                TotalCancelledOrders = totalCancelledOrders
            };
        }
        public async Task<OrderStatsDTO> GetOrderStats()
        {
            var totalOrders = await GetTotalOrdersCountAsync();
            var totalPendingOrders = await GetTotalOrdersCountByStatusAsync(OrderStatus.Pending);
            var totalAcceptedOrders = await GetTotalOrdersCountByStatusAsync(OrderStatus.Accepted);
            var totalShippedOrders = await GetTotalOrdersCountByStatusAsync(OrderStatus.Shipped);
            var totalDeliveredOrders = await GetTotalOrdersCountByStatusAsync(OrderStatus.Delivered);
            var totalCancelledOrders = await GetTotalOrdersCountByStatusAsync(OrderStatus.Cancelled);

            return new OrderStatsDTO
            {
                TotalOrders = totalOrders,
                TotalPendingOrders = totalPendingOrders,
                TotalAcceptedOrders = totalAcceptedOrders,
                TotalShippedOrders = totalShippedOrders,
                TotalDeliveredOrders = totalDeliveredOrders,
                TotalCancelledOrders = totalCancelledOrders
            };
        }

        #endregion

        // **************************************** End Of Hamed ****************************************
    }
}
