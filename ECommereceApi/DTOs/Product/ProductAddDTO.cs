using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommereceApi.DTOs.Product
{
    public class ProductAddDTO
    {
        public string Name { get; set; }
        [Range(minimum: 0, maximum: Double.MaxValue)]
        public double? Discount { get; set; }
        [Range(minimum: 0, maximum: Double.MaxValue, MinimumIsExclusive = true)]
        public double OriginalPrice { get; set; }
        [Range(minimum: 0, maximum: Int32.MaxValue, MinimumIsExclusive = true)]
        public int Amount { get; set; }

        public string Description { get; set; }
        [Range(minimum: 0, maximum: Int32.MaxValue, MinimumIsExclusive = true)]
        public int CategoryId { get; set; }
        //public HashSet<int> SubCategoriesIds { get; set; } = new HashSet<int>();
        //public List<string> SubCategoriesValues { get; set; } = new List<string>();
    }
}
