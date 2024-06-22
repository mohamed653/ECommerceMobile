namespace ECommereceApi.Services.Interfaces
{
    public interface IFileCloudService
    {
        string GetImageUrl(string publicId);
        string getPublicId(string url);
        Task<bool> DeleteImageAsync(string publicId);
        Task<string> UploadImagesAsync(IFormFile picture);
        Task<string> UpdateImageAsync(IFormFile picture,string publicId);
    }
}
