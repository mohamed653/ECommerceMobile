using System.ComponentModel.DataAnnotations.Schema;

namespace ECommereceApi.Models
{
	[Table("ProductOrder")]
	public class ProductOrder
	{
		public int OrderId { get; set; }
		[ForeignKey("OrderId")]
		public virtual Order Order { get; set; }

		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		public virtual Product Product { get; set; }
	}
}
