using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommereceApi.Models
{
    [Index(nameof(CategoryId), nameof(SubCategoryId), IsUnique = true)]
    public class CategorySubCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategorySubCategoryId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("SubCategory")]
        public int SubCategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual ICollection<CategorySubCategoryValues> CategorySubCategoryValues { get; set; } = new List<CategorySubCategoryValues>();
    }
}
