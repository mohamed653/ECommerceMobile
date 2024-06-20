namespace ECommereceApi.DTOs.Product
{
    public class CategoryAddDTO
    {
        public string Name { get; set; }
        public HashSet<int> SubCategoriesIds { get; set; } = new HashSet<int>();
        public IFormFile? Image { get; set; }
    }
}
