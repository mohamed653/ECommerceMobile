namespace ECommereceApi.DTOs.Product
{
    public class SubCategoryValuesDTO
    {
        public int SubCategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<SubCategoryValuesDetailsDTO> Values { get; set; } = new List<SubCategoryValuesDetailsDTO>();
    }
}
