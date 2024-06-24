using AutoMapper;
using ECommereceApi.DTOs.Account;
using ECommereceApi.DTOs.Order;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Models;
using ECommereceApi.Services.classes;
using ECommereceApi.Services.Interfaces;

namespace ECommereceApi.Services.Mapper
{
    public class MyMapperConfiguration : Profile
    {
        //private readonly IFileCloudService _fileCloudServer;
        public MyMapperConfiguration()
        {

            //_fileCloudServer = fileCloudService;

            CreateMap<Product, ProductDisplayDTO>()
                .ForMember(p => p.CategoryName, option => option.MapFrom(p => p.Category.Name))
                .ReverseMap();

            CreateMap<ProductDisplayDTO, ProductDisplayInCartDTO>().ReverseMap();

            CreateMap<ProductImage, ProductImageDTO>()
                .ForMember(p => p.ImageUri, option => option.MapFrom<ImageUriCustomResolver>())
                .ReverseMap();

            CreateMap<Product, ProductAddDTO>().ReverseMap();

            CreateMap<Category, CategoryDTO>()
                .ForMember(c => c.SubCategories, opt => opt.MapFrom(sc => sc.Subs.Select(s => s.SubCategory)))
                .ReverseMap();

            CreateMap<SubCategory, SubCategoryDTO>()
                .ForMember(dest => dest.SubCategoryId, option => option.MapFrom(src => src.SubId))
                .ReverseMap();

            CreateMap<SubCategoryAddDTO, SubCategory>()
                .ReverseMap();

            CreateMap<CategorySubCategoryValues, ProductCategorySubCategoryValuesDTO>()
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.CategorySubCategory.Category.Name))
                .ForMember(d => d.SubCategoryName, opt => opt.MapFrom(s => s.CategorySubCategory.SubCategory.Name))
                .ReverseMap();

            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserDTOUi>().ReverseMap();

            // Offers
            CreateMap<Offer, OfferDTO>().ReverseMap();
            CreateMap<OffersDTOUI, Offer>().ReverseMap();
            CreateMap<ProductOffer, OfferProductsDetailedDTO>().ReverseMap();
            CreateMap<OfferProductsDetailedDTO, ProductOffer>().ReverseMap();

            //WebInfo
            CreateMap<ECommereceApi.Models.WebInfo, WebInfoDTO>().ReverseMap();

            #region Orders
            CreateMap<OrderPreviewDTO, CartProductsDTO>().ReverseMap();
            CreateMap<OfferDisplayDTO, Offer>().ReverseMap();
            #endregion

        }
    }
}
