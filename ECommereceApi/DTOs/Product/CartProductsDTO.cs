namespace ECommereceApi.DTOs.Product
{
    public class CartProductsDTO
    {
        public int UserId { get; set; }
        public List<ProductDisplayDTO> ProductsAmounts { get; set; } = new();
        public double FinalPrice { get; set; }
        public int numberOfUniqueProducts { get; set; } = 0;
        public int numberOfProducts { get; set; } = 0;
    }
}
