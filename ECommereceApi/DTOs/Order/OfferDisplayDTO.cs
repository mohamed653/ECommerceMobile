using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Order
{
    public class OfferDisplayDTO
    {
        public int OfferId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }

        public string Description { get; set; }

        public DateOnly OfferDate { get; set; }

        public int? Duration { get; set; }

        public decimal? PackageDiscount { get; set; }
    }
}
