using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Product
{
    public class OfferDTO
    {

        // Offer properties
        public string Title { get; set; }

        public string Description { get; set; }

        public DateOnly OfferDate { get; set; }

        public int? Duration { get; set; }

        public decimal? PackageDiscount { get; set; }

        // Plus Image

        public IFormFile Image { get; set; }
    }

    public class OfferProductsDTO
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }

        public string? Image { get; set; }
        public int ProductAmount { get; set; }

        public double? Discount { get; set; }


    }
    //public class OfferProductsDetailedDTO
    //{
    //    public string Name { get; set; }

    //    public string? Image { get; set; }
    //}


    public class OffersDTOUI
    {
        public int OfferId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public DateOnly OfferDate { get; set; }

        public int? Duration { get; set; }
        public string Image { get; set; }

        public decimal? PackageDiscount { get; set; }
        public List<OfferProductsDTO> ProductOffers { get; set; } = new List<OfferProductsDTO>();
    }
}