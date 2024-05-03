namespace ECommereceApi.DTOs
{
    public class ProductPictureDTO
    {
        public int ProductId { get; set; }
        public List<IFormFile> Pictures { get; set; }
    }
}
