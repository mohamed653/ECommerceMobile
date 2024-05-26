namespace ECommereceApi.DTOs.Product
{
    public class ProductPictureDTO
    {
        public int ProductId { get; set; }
        public List<IFormFile> Pictures { get; set; }
    }
}
