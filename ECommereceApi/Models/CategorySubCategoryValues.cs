using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommereceApi.Models
{
    [PrimaryKey("CategorySubCategoryId", "Value")]
    public class CategorySubCategoryValues
    {
        [ForeignKey("CategorySubCategory")]
        public int CategorySubCategoryId { get; set; }
        public string Value { get; set; }
        public string? ImageUri { get; set; }
        public virtual CategorySubCategory CategorySubCategory { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
