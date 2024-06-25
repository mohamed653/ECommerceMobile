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
        public string GetImageUrl(string? publicId)
        {
            if(publicId is null) return null;
            var getParams = new GetResourceParams(publicId);

            var result = _cloudinary.GetResource(getParams);
            // Return the URL of the retrieved image
            return result.SecureUrl;
        }

        public string getPublicId(string url)
        {
            var uri = new Uri(url);
            var publicId = uri.Segments[^1].Split('.')[0];
            return publicId;
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);
            return result.Result == "ok";
        }

        public async Task<string> UploadImagesAsync(IFormFile picture)
        {
            if (picture.Length <= 0) return null;

            await using var stream = picture.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(picture.FileName, stream)
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.PublicId;
        }

        public async Task<string> UpdateImageAsync(IFormFile picture, string publicId)
        {
            var deletionResult = await DeleteImageAsync(publicId);
            if (!deletionResult) return null;

            var newPublicId = await UploadImagesAsync(picture);
            return newPublicId;
        }

        //public string GetImageUrl(string publicId)
        //{
        //    var getParams = new GetResourceParams(publicId);

        //    var result = _cloudinary.GetResource(getParams);
        //    // Return the URL of the retrieved image
        //    return result.SecureUrl;
        //}
        //public string getPublicId(string url)
        //{
        //    var uri = new Uri(url);
        //    var publicId = uri.Segments[^1].Split('.')[0];
        //    return publicId;
        //}
        //public async Task<bool> DeleteImageAsync(string publicId)
        //{
        //    var delParams = new DeletionParams(publicId);

        //    var result = await _cloudinary.DestroyAsync(delParams);

        //    // Return true if deletion was successful, false otherwise
        //    return result.Result == "ok";
        //}
        //public async Task<string> UploadImagesAsync(IFormFile picture)
        //{
        //    if (picture == null || picture.Length == 0)
        //    {
        //        return "Invalid picture";
        //    }

        //    var uploadParams = new ImageUploadParams()
        //    {
        //        File = new FileDescription(picture.FileName, picture.OpenReadStream())
        //    };

        //    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        //    // Handle the result as needed, e.g., save the image URL to your database
        //    var imagePublicId = uploadResult.PublicId;

        //    return imagePublicId;
        //}
        //public async Task<string> UpdateImageAsync(IFormFile picture,string publicId)
        //{
        //    if (picture == null || picture.Length == 0)
        //    {
        //        return "Invalid picture";
        //    }

        //    var delParams = new DeletionParams(publicId);
        //    var result = await _cloudinary.DestroyAsync(delParams);
        //    // upload new image
        //    var uploadParams = new ImageUploadParams()
        //    {
        //        File = new FileDescription(picture.FileName, picture.OpenReadStream())
        //    };
        //    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        //    var imageUrl = uploadResult.Url.ToString();
        //    return imageUrl;
        //}


    }
}
