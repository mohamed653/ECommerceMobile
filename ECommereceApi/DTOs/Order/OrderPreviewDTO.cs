using ECommereceApi.DTOs.Product;

namespace ECommereceApi.DTOs.Order
{
    public class OrderPreviewDTO : CartProductsDTO
    {
        public List<OfferDisplayDTO> ApplicableOffers { get; set; }
    }
}
