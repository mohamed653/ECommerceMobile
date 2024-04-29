using System.ComponentModel.DataAnnotations.Schema;

namespace ECommereceApi.Models
{
	[Table("WishList")]
	public class WishList
	{
		public int UserId { get; set; }
		[ForeignKey("UserId")]
		public virtual User User { get; set; }

		public int ProductId { get; set; }

		[ForeignKey("ProductId")]
		public virtual Product Product { get; set; }
	}

}
