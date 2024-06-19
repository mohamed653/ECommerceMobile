using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Product
{
    public class ProductImageDTO
    {
    public int ProductId { get; set; }
    [StringLength(200)]
    public string ImageUri { get; set; }
    public string ImageId{ get; set; }
    }
}
