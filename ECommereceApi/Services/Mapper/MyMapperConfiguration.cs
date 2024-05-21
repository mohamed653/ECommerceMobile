using AutoMapper;
using ECommereceApi.DTOs;
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
            CreateMap<CategoryDTO, Category>().ReverseMap();

            CreateMap<SubCategory, CategoryDTO>()
                .ForMember(dest => dest.CategoryId, option => option.MapFrom(src => src.SubId));

            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserDTOUi>().ReverseMap();
            CreateMap<Offer, OfferDTO>().ReverseMap();
            CreateMap<OffersDTOUI,Offer > ().ReverseMap();
            CreateMap<OfferProductsDTO, ProductOffer>().ReverseMap();
            CreateMap<ProductOffer, OfferProductsDTO>().ReverseMap();


        }
    }
}
