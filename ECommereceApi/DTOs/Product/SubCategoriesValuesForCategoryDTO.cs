namespace ECommereceApi.DTOs.Product
{
    public class SubCategoriesValuesForCategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<SubCategoryValuesDTO> SubCategories { get; set; }
    }
}
