namespace ECommereceApi.DTOs.Product
{
    public class CategorySubCategoryValuesAddDTO
    {
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Value { get; set; }
        public IFormFile? Image { get; set; }
    }
}
