using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommereceApi.Models
{
    [PrimaryKey(nameof(ProductId), nameof(CategorySubCategoryValuesId))]
    public class ProductCategorySubCategoryValues
    {
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        [ForeignKey(nameof(CategorySubCategoryValues))]
        public int CategorySubCategoryValuesId { get; set; }
        public virtual CategorySubCategoryValues CategorySubCategoryValues { get; set; }
        public virtual Product Product { get; set; }
    }
}
