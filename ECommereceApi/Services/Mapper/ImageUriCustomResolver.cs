using AutoMapper;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Services.Interfaces;

namespace ECommereceApi.Services.Mapper
{
    public class ImageUriCustomResolver : IValueResolver<ProductImage, ProductImageDTO, string>
    {
        private readonly IFileCloudService _fileCloudService;
        public ImageUriCustomResolver(IFileCloudService fileCloudService)
        {
            _fileCloudService = fileCloudService;
        }
        public string Resolve(ProductImage source, ProductImageDTO destination, string destMember, ResolutionContext context)
        {
                return _fileCloudService.GetImageUrl(source.ImageId);
        }
    }
}
