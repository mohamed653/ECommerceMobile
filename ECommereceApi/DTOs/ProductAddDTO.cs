using System.ComponentModel.DataAnnotations.Schema;

namespace ECommereceApi.DTOs
{
    public class ProductAddDTO
    {
        public string Name { get; set; }

        public double? Discount { get; set; }

        public double OriginalPrice { get; set; }

        public int Amount { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }
    }
}
