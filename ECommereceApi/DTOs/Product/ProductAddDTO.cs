using System.ComponentModel.DataAnnotations.Schema;

namespace ECommereceApi.DTOs.Product
{
    public class ProductAddDTO
    {
        public string Name { get; set; }

        public double? Discount { get; set; }

        public double OriginalPrice { get; set; }

        public int Amount { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }
        //public HashSet<int> SubCategoriesIds { get; set; } = new HashSet<int>();
        //public List<string> SubCategoriesValues { get; set; } = new List<string>();
    }
}
