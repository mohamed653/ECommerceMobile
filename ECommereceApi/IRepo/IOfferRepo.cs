namespace ECommereceApi.IRepo
{
    public interface IOfferRepo
    {
        Task<OffersDTOUI> GetOfferById(int id);
        Task<List<Offer>> GetOffers();
        Task<List<OffersDTOUI>> GetOffersWithProducts();
        Task<int> AddOffer(OfferDTO offerDTO);
        Task AddProductsToOffer(int offerId, List<OfferProductsDTO> offerProductsDTOs,decimal? PackageDiscount);

        Task DeleteOffer(int offerId);
        Task <bool> OfferExpiredOrInActive(int offerId);

        Task UpdateOffer(OffersDTOUI offersDTOUI );
    }
}
