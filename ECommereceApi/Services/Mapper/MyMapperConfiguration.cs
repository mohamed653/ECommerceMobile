using AutoMapper;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Models;

namespace ECommereceApi.Services.Mapper
{
    public class MyMapperConfiguration : Profile
    {
        public MyMapperConfiguration()
        {
            CreateMap<Product, ProductDisplayDTO>()
                .ForMember(p => p.CategoryName, option => option.MapFrom(p => p.Category.Name))
                .ReverseMap();

            CreateMap<Product, ProductAddDTO>().ReverseMap();
            CreateMap<CategoryDTO, Category>()
                .ForMember(c => c.Subs, options => options.MapFrom(dest => dest.SubCategories))
                .ReverseMap();

            CreateMap<SubCategory, SubCategoryDTO>()
                .ForMember(dest => dest.SubCategoryId, option => option.MapFrom(src => src.SubId))
                .ReverseMap();

            CreateMap<SubCategoryAddDTO, SubCategory>()
                .ReverseMap();

            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserDTOUi>().ReverseMap();
            CreateMap<Offer, OfferDTO>().ReverseMap();
            CreateMap<OffersDTOUI,Offer > ().ReverseMap();
            CreateMap<OfferProductsDTO, ProductOffer>().ReverseMap();
            CreateMap<ProductOffer, OfferProductsDTO>().ReverseMap();
            CreateMap<ECommereceApi.Models.WebInfo, WebInfoDTO>().ReverseMap();

        }
    }
}
