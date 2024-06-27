using ECommereceApi.Enums;

namespace ECommereceApi.DTOs.Order
{
    public class OrderPostDTO
    {
        public int OfferId { get; set; }
        public int UserId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string Governerate { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }

    }
}
