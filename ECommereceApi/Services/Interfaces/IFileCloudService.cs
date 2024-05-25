namespace ECommereceApi.Services.Interfaces
{
    public interface IFileCloudService
    {
        string GetImageUrl(string publicId);
        Task<bool> DeleteImage(string publicId);
        Task<string> UploadImages(IFormFile picture);
        Task<string> UpdateImage(IFormFile picture,string publicId);
    }
}
