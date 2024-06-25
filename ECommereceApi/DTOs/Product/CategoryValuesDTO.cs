namespace ECommereceApi.DTOs.Product
{
    public class CategoryValuesDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<SubCategoryValuesDetailsDTO> Values { get; set; } = new List<SubCategoryValuesDetailsDTO>();
    }
}
