using ECommereceApi.DTOs.Product;

namespace ECommereceApi.DTOs.Order
{
    public class OrderPreviewWithoutOffersDTO
    {
        public int UserId { get; set; }
        public Guid OrderId { get; set; }
        public List<ProductDisplayInCartDTO> ProductsAmounts { get; set; } = new();
        public double FinalPrice { get; set; }
        public int numberOfUniqueProducts { get; set; } = 0;
        public int numberOfProducts { get; set; } = 0;
        public string Governerate { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
    }
}
