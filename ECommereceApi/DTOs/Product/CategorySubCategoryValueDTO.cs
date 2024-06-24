namespace ECommereceApi.DTOs.Product
{
    public class CategorySubCategoryValueDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Value { get; set; }
        public string ImageId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
