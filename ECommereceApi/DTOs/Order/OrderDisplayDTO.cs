using ECommereceApi.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Order
{
    public class OrderDisplayDTO
    {
        public Guid OrderId { get; set; }

        public string Governerate { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public DateOnly? ArrivalDate { get; set; }

        public int UserId { get; set; }

        public OrderStatus Status { get; set; }

        public virtual ICollection<ProductOrderDTO> ProductOrders { get; set; }
        public int? OfferId { get; set; }

        public DateOnly? ResponseDate { get; set; }

        public double? TotalPrice { get; set; }
        public int? TotalAmount { get; set; }

        public PaymentMethod? PaymentMethod{ get; set; }

    }

    public class ProductOrderDTO
    {
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }
    }
}

