using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class OfferRepo : IOfferRepo
    {
        private readonly ECommerceContext _context;
        private readonly Cloudinary _cloudinary;
        private readonly IMapper _mapper;
        public OfferRepo(ECommerceContext context, Cloudinary cloudinary, IMapper mapper)
        {
            _context = context;
            _cloudinary = cloudinary;
            _mapper = mapper;
        }

        public async Task<int> AddOffer(OfferDTO offerDTO)
        {
            try
            {
                var offer = _mapper.Map<Offer>(offerDTO);
                await _context.Offers.AddAsync(offer);
                await _context.SaveChangesAsync();
                //return the created offer ID
                return offer.OfferId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task AddProductsToOffer(int offerId, List<OfferProductsDTO> offerProductsDTOs, decimal? PackageDiscount)
        {
            try
            {
                
                var offer = await _context.Offers.Include(x => x.ProductOffers).FirstOrDefaultAsync(x => x.OfferId == offerId);
                if (offer == null)
                    throw new Exception("Offer not found");

                var productOffer = _mapper.Map<List<ProductOffer>>(offerProductsDTOs);
                offer.PackageDiscount = PackageDiscount;
                foreach (var item in productOffer)
                {
                    item.OfferId = offerId;
                    offer.ProductOffers.Add(item);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<OffersDTOUI>> GetOffersWithProducts()
        {
            try
            {
                var offers = await _context.Offers.Include(x => x.ProductOffers).ToListAsync();
                var offersDTO = _mapper.Map<List<OffersDTOUI>>(offers);
                foreach (var offer in offersDTO)
                {
                    offer.ProductOffers = _mapper.Map<List<OfferProductsDTO>>(offers.FirstOrDefault(x => x.OfferId == offer.OfferId).ProductOffers);
                }
                return offersDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteOffer(int offerId)
        {
            try
            {
                var offer = await _context.Offers.FindAsync(offerId);
                if (offer == null)
                    throw new Exception("Offer not found");

                _context.Offers.Remove(offer);

                var offerProducts = await _context.ProductOffers.Where(x => x.OfferId == offerId).ToListAsync();
                _context.ProductOffers.RemoveRange(offerProducts);


                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OffersDTOUI> GetOfferById(int id)
        {
            try
            {
                var offer = await _context.Offers.Include(x => x.ProductOffers).FirstOrDefaultAsync(x => x.OfferId == id);
                if (offer == null)
                    throw new Exception("Offer not found");

                var offerDTO = _mapper.Map<OffersDTOUI>(offer);
                offerDTO.ProductOffers = _mapper.Map<List<OfferProductsDTO>>(offer.ProductOffers);


                return offerDTO;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task<List<Offer>> GetOffers()
        {
            return await  _context.Offers.ToListAsync();
        }

        public async Task<string> UploadImages(IFormFile picture)
        {
            if (picture == null || picture.Length == 0)
            {
                return "Invalid picture";
            }

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(picture.FileName, picture.OpenReadStream())
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Handle the result as needed, e.g., save the image URL to your database
            var imageUrl = uploadResult.Url.ToString();

            return imageUrl;
        }

        public async Task<bool> OfferExpiredOrInActive(int offerId)
        {
            var offer = _context.Offers.Find(offerId);

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

        public async Task UpdateOffer(OffersDTOUI offersDTOUI)
        {
            try
            {
                var offer = await _context.Offers.Include(x => x.ProductOffers).FirstOrDefaultAsync(x => x.OfferId == offersDTOUI.OfferId);
                if (offer == null)
                    throw new Exception("Offer not found");

                _mapper.Map(offersDTOUI, offer);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
