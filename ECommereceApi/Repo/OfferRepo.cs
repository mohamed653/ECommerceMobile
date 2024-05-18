using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class OfferRepo:IOfferRepo
    {
        private readonly ECommerceContext _context;
        public OfferRepo(ECommerceContext context)
        {
            _context = context;
        }

        public async Task AddOffer(Offer offer, int productId)
        {
            await _context.Offers.AddAsync(offer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOffer(Offer offer)
        {
            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();
        }

        public async Task<Offer> GetOfferById(int id)
        {
            return await _context.Offers.FindAsync(id);
        }

        public async Task<IEnumerable<Offer>> GetOffers()
        {
            return await _context.Offers.ToListAsync();
        }

        public async Task UpdateOffer(Offer offer)
        {
            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();
        }

        public bool OfferExpiredOrInActive(int id)
        {
            var offer = _context.Offers.Find(id);

            // Check if the offer is null
            if (offer == null)
                return true;

            // Calculate the end date of the offer
            var startDate = offer.OfferDate;
            int durationInDays = offer.Duration ?? 0; // Assuming a duration of 0 if it is null

            if (DateTime.Now > startDate.ToDateTime(TimeOnly.MinValue).AddDays(durationInDays))
                return true;

            return false;
        }

       
    }
}
