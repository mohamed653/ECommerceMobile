using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommereceApi.Models
{
	[Table("ProductOrder")]
	[PrimaryKey(nameof(OrderId), nameof(ProductId))]
	public class ProductOrder
	{
		public Guid OrderId { get; set; }
		[ForeignKey("OrderId")]
		public virtual Order Order { get; set; }

		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		public virtual Product Product { get; set; }
        public int Amount { get; set; }
    }
}
