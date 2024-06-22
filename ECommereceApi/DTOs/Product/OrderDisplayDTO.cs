namespace ECommereceApi.DTOs.Product
{
    public class OrderDisplayDTO : CartProductsDTO
    {
        public int? OfferId { get; set; }
        public double FinalPriceWithOffer { get; set; }
    }
}
