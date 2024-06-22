using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Product
{

    // a DTO for the Offer model  GENERAL
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



    // a DTO for OfferProducts when  POST
    public class OffersDTOPost
    {
        public int ProductId { get; set; }
        public int ProductAmount { get; set; }
        public double? Discount { get; set; }
    }

    public class UpdateProdDTO
    {
        public string ProductName { get; set; }
        public string ProductImage { get; set; }


    }

    // a DTO for the OfferProducts when GET
    public class OfferProductsDetailedDTO
    {

        public int ProductId { get; set; }

        public int ProductAmount { get; set; }

        public double? Discount { get; set; }

        public string? Name { get; set; }

        public string? Image { get; set; }
    }

    // a DTO for OfferProducts List when  GET
    public class OffersDTOUI
    {
        public int OfferId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public DateOnly OfferDate { get; set; }

        public int? Duration { get; set; }
        public string Image { get; set; }

        public decimal? PackageDiscount { get; set; }
        public List<OfferProductsDetailedDTO> ProductOffers { get; set; } = new List<OfferProductsDetailedDTO>();
    }
}