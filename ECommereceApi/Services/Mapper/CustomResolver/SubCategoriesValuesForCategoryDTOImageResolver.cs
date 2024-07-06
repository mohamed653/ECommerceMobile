using AutoMapper;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Services.Interfaces;

namespace ECommereceApi.Services.Mapper.CustomResolver
{
    public class SubCategoriesValuesForCategoryDTOImageResolver : IValueResolver<Category, SubCategoriesValuesForCategoryDTO, string>
    {
        private readonly IFileCloudService _fileCloudService;
        public SubCategoriesValuesForCategoryDTOImageResolver(IFileCloudService fileCloudService)
        {
            _fileCloudService = fileCloudService;
        }

        public string Resolve(Category source, SubCategoriesValuesForCategoryDTO destination, string destMember, ResolutionContext context)
        {
            return _fileCloudService.GetImageUrl(source.ImageId);
        }
    }
}
