﻿using ECommereceApi.DTOs.Product;
using ECommereceApi.Repo;

namespace ECommereceApi.IRepo
{
    public interface IOfferRepo
    {
        Task<OffersDTOUI> GetOfferById(int id);
        Task<List<Offer>> GetOffers();
        Task<List<OffersDTOUI>> GetOffersWithProducts();

        Task<List<Offer>> GetOffersByProductId(int productId);
        //Task<Status> UpdateProductsFromOffer(int offerId, OffersDTOPost offerProductsDTO);
        Task<Status> UpdateProductsFromOffer(int offerId, ProductAddDTO productAddDTO, int productId);
        Task<int> AddOffer(OfferDTO offerDTO);
        Task AddProductsToOffer(int offerId, OffersDTOPost offerProductsDTO);
        Task UpdateOffer(int offerId,OfferDTO offerDTO);
        Task DeleteOffer(int offerId);
        Task <bool> OfferExpiredOrInActive(int offerId);

        Task<List<OffersDTOUI>> RemoveProductFromOffer(int offerId, int productId);
       

    }
}
