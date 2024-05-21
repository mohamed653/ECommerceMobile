using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using ECommereceApi.Services.Interfaces;

namespace ECommereceApi.Services.classes
{
    public class FileCloudService: IFileCloudService
    {

        private readonly Cloudinary _cloudinary;
        public FileCloudService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public string GetImageUrl(string publicId)
        {
            var getParams = new GetResourceParams(publicId);

            var result = _cloudinary.GetResource(getParams);

            // Return the URL of the retrieved image
            return result.SecureUrl;
        }
        public async Task<bool> DeleteImage(string publicId)
        {
            var delParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(delParams);

            // Return true if deletion was successful, false otherwise
            return result.Result == "ok";
        }
        public async Task<string> UploadImages(IFormFile picture)
        {
            if (picture == null || picture.Length == 0)
            {
                return "Invalid picture";
            }

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(picture.FileName, picture.OpenReadStream())
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Handle the result as needed, e.g., save the image URL to your database
            var imageUrl = uploadResult.Url.ToString();

            return imageUrl;
        }
        public async Task<string> UpdateImage(IFormFile picture,string publicId)
        {
            if (picture == null || picture.Length == 0)
            {
                return "Invalid picture";
            }

            var delParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(delParams);
            // upload new image
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(picture.FileName, picture.OpenReadStream())
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            var imageUrl = uploadResult.Url.ToString();
            return imageUrl;
        }


    }
}
