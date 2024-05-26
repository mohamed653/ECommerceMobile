using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Product
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public IEnumerable<SubCategoryDTO> SubCategories { get; set; } = new HashSet<SubCategoryDTO>();
    }
}
