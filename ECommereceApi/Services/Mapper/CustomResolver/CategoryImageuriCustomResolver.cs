using AutoMapper;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Services.Interfaces;

namespace ECommereceApi.Services.Mapper.CustomResolver
{
    public class CategoryImageuriCustomResolver : IValueResolver<Category, CategoryDTO, string>
    {
        private readonly IFileCloudService _fileCloudService;
        public CategoryImageuriCustomResolver(IFileCloudService fileCloudService)
        {
            _fileCloudService = fileCloudService;
        }
        public string Resolve(Category source, CategoryDTO destination, string destMember, ResolutionContext context)
        {
            return _fileCloudService.GetImageUrl(source.ImageId);
        }
    }
}
