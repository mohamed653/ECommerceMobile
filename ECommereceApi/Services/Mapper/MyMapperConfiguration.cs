using AutoMapper;
using ECommereceApi.DTOs.Account;
using ECommereceApi.DTOs.Offer;
using ECommereceApi.DTOs.Order;
using ECommereceApi.DTOs.Product;
using ECommereceApi.DTOs.WebInfo;
using ECommereceApi.Models;
using ECommereceApi.Services.classes;
using ECommereceApi.Services.Interfaces;
using ECommereceApi.Services.Mapper.CustomResolver;

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
                .ForMember(p => p.ProductImages, option => option.MapFrom(p => p.ProductImages)) 
                .ReverseMap();

            CreateMap<ProductImage, ProductImageDTO>()
                .ReverseMap();

            CreateMap<Category, SubCategoryValuesDTO>();

            CreateMap<ProductDisplayDTO, ProductDisplayInCartDTO>().ReverseMap();

            CreateMap<ProductImage, ProductImageDTO>()
                .ForMember(p => p.ImageUri, option => option.MapFrom<ImageUriCustomResolver>())
                .ReverseMap();

            CreateMap<Product, ProductAddDTO>().ReverseMap();

            CreateMap<Category, CategoryDTO>()
                .ForMember(c => c.SubCategories, opt => opt.MapFrom(sc => sc.CategorySubCategory.Select(s => s.SubCategory)))
                .ForMember(c => c.ImageUri, option => option.MapFrom<CategoryImageuriCustomResolver>())
                .ReverseMap();

            CreateMap<SubCategory, SubCategoryDTO>()
                .ForMember(dest => dest.SubCategoryId, option => option.MapFrom(src => src.SubCategoryId))
                .ReverseMap();

            CreateMap<SubCategoryAddDTO, SubCategory>()
                .ReverseMap();

            CreateMap<SubCategoryValuesDetailsDTO, CategorySubCategoryValues>().ReverseMap();

            CreateMap<CategorySubCategoryValues, ProductCategorySubCategoryValuesDTO>()
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.CategorySubCategory.Category.Name))
                .ForMember(d => d.SubCategoryName, opt => opt.MapFrom(s => s.CategorySubCategory.SubCategory.Name))
                .ReverseMap();
            
            CreateMap<Category, SubCategoriesValuesForCategoryDTO>()
                .ForMember(c => c.ImageUri, option => option.MapFrom<SubCategoriesValuesForCategoryDTOImageResolver>())
                .ReverseMap();

            CreateMap<CategorySubCategoryValues, SubCategoryValuesDetailsDTO>()
                .ForMember(csv => csv.ImageUrl, option => option.MapFrom<CategorySubCategoryValuesImageResolver>())
                .ReverseMap();

            CreateMap<CategoriesValuesForSubCategoryDTO, SubCategory>().ReverseMap();

            CreateMap<SubCategoryValuesDTO, SubCategory>().ReverseMap();

            CreateMap<CategoryValuesDTO, Category>().ReverseMap();

            CreateMap<CategorySubCategoryValueDTO, CategorySubCategoryValuesAddDTO>()
                .ReverseMap();

            // Users
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserDTOUi>().ReverseMap();
            CreateMap<User,UserUpdateDTO>().ReverseMap();

            // Offers
            CreateMap<Offer, OfferDTO>().ReverseMap();
            CreateMap<OffersDTOUI, Offer>().ReverseMap();
            CreateMap<ProductOffer, OfferProductsDetailedDTO>().ReverseMap();
            CreateMap<OfferProductsDetailedDTO, ProductOffer>().ReverseMap();

            // WebInfo
            CreateMap<ECommereceApi.Models.WebInfo, WebInfoDTO>().ReverseMap();

            #region Orders
            CreateMap<OrderPreviewDTO, CartProductsDTO>().ReverseMap();
            CreateMap<OfferDisplayDTO, Offer>().ReverseMap();
            CreateMap<OrderPreviewWithoutOffersDTO, AddOrderOfferDTO>().ReverseMap();
            CreateMap<Order, AddOrderOfferDTO>().ReverseMap();
            CreateMap<OrderDisplayDTO, Order>().ReverseMap();

            #endregion

        }
    }
}
