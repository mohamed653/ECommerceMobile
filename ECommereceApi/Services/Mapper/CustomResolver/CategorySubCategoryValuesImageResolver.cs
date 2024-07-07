using AutoMapper;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Services.Interfaces;

namespace ECommereceApi.Services.Mapper.CustomResolver
{
    public class CategorySubCategoryValuesImageResolver : IValueResolver<CategorySubCategoryValues, SubCategoryValuesDetailsDTO, string>
    {
        private IFileCloudService _fileCloudService;
        public CategorySubCategoryValuesImageResolver(IFileCloudService fileCloudService)
        {
            _fileCloudService = fileCloudService;
        }

        public string Resolve(CategorySubCategoryValues source, SubCategoryValuesDetailsDTO destination, string destMember, ResolutionContext context)
        {
            return _fileCloudService.GetImageUrl(source.ImageId);
        }
    }
}
