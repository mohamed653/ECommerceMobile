using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommereceApi.Models
{
    [Index(nameof(CategorySubCategoryId), nameof(Value), IsUnique = true)]
    public class CategorySubCategoryValues
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("CategorySubCategory")]
        public int CategorySubCategoryId { get; set; }
        public string Value { get; set; }
        public string? ImageUri { get; set; }
        public string? ImageId { get; set; }
        public virtual CategorySubCategory CategorySubCategory { get; set; }
        public virtual ICollection<ProductCategorySubCategoryValues> ProductCategorySubCategoryValues { get; set; } = new List<ProductCategorySubCategoryValues>();
    }
}
