namespace ECommereceApi.IRepo
{
    public interface IOfferRepo
    {
        Task<Offer> GetOfferById(int id);
        Task<IEnumerable<Offer>> GetOffers();
        Task AddOffer(Offer offer,int productId);
        Task UpdateOffer(Offer offer);
        Task DeleteOffer(Offer offer);
        bool OfferExpiredOrInActive(int id);
    }
}
