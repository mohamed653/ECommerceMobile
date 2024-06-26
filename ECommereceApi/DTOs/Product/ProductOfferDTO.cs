using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Product
{

    // a DTO for the Offer model  GENERAL
    public class OfferDTO
    {

        // Offer properties
        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, ErrorMessage = "Title should be less than 50 characters")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Offer Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly OfferDate { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 365, ErrorMessage = "Duration should be between 1 and 365")] 
        public int Duration { get; set; }

        // min value is 0 and max value unrestricted
        [Range(0, double.MaxValue, ErrorMessage = "Package Discount can't be applied")]
        public decimal? PackageDiscount { get; set; }

        // Plus Image
        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; set; }
    }



    // a DTO for OfferProducts when  POST
    public class OffersDTOPost
    {
        public int ProductId { get; set; }
        [DefaultValue(1)]
        [Range(1,int.MaxValue, ErrorMessage = "Product Amount should be more than 0")]
        public int ProductAmount { get; set; }

        [DefaultValue(0)]
        [Range(0, 100, ErrorMessage = "Discount Should be between 0 and 100")]
        public double? Discount { get; set; } = 0;
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