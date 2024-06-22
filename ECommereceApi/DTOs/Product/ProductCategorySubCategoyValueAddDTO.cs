namespace ECommereceApi.DTOs.Product
{
    public class ProductCategorySubCategoyValueAddDTO
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Value { get; set; }
        public IFormFile? Image { get; set; }
    }
}
