
using ECommereceApi.DTOs.Product;
using ECommereceApi.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ECommereceApi.DTOs.Order
{
    public class OrderDisplayDTO
    {
        public Guid OrderId { get; set; }

        public string Governerate { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

    
        public int UserId { get; set; }
        public virtual UserOrderDTO User { get; set; }

        public OrderStatus Status { get; set; }

        public virtual ICollection<ProductOrderDTO> ProductOrders { get; set; }
        public int? OfferId { get; set; }
        public DateOnly? ArrivalDate
        {
            get
            {
                if (ShippingDate.HasValue && ArrivalInDays.HasValue)
                {
                    return ShippingDate.Value.AddDays(ArrivalInDays.Value);
                }
                return null;
            }
        }
        public DateOnly? ShippingDate { get; set; }

        public int? ArrivalInDays { get; set; }

        public double? TotalPrice { get; set; }
        public int? TotalAmount { get; set; }

        public PaymentMethod? PaymentMethod{ get; set; }
        public decimal? Score { get; set; }

    }

    public class ProductOrderDTO
    {
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }

        public string ProductName
        {
            get { return Product?.Name; }
            private set { } 
        }

        public double? ProductPrice
        {
            get { return Product?.FinalPrice; }
            private set { }
        }

        public string ProductImageUri
        {
            get { return ProductImage?.ImageUri; }
            private set { }
        }

        [JsonIgnore]
        public ProductImageDTO ProductImage{ get; set; }

        [JsonIgnore]
        public virtual Models.Product Product { get; set; }
    }
    public class UserOrderDTO
    {
        public string FName { get; set; }
        public string LName { get; set; }

        public string FullName
        {
            get { return FName + " " + LName; }
            set {  }
        }
        public string Email { get; set; }

        public string Phone { get; set; }
    }
}

