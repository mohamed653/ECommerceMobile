using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Product
{
    public class CategoriesValuesForSubCategoryDTO
    {
        public int SubCategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<CategoryValuesDTO> Categories { get; set; }
    }
}
