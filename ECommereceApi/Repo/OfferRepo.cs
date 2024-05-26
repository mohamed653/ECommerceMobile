using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class OfferRepo : IOfferRepo
    {
        private readonly ECommerceContext _context;
        private readonly IFileCloudService _fileCloudService;
        private readonly IMapper _mapper;
        public OfferRepo(ECommerceContext context, IFileCloudService fileCloudService, IMapper mapper)
        {
            _context = context;
            _fileCloudService = fileCloudService;
            _mapper = mapper;
        }
        public async Task<List<Offer>> GetOffers()
        {
            return await _context.Offers.ToListAsync();
        }
        public async Task<List<OffersDTOUI>> GetOffersWithProducts()
        {
            try
            {
                // if the offer is expired or inactive don't include it int the list

                var offers = await _context.Offers.Include(x => x.ProductOffers).ToListAsync();
                offers = offers.Where(x => !OfferExpiredOrInActive(x)).ToList();

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

        public async Task<List<Offer>> GetOffersByProductId(int productId)
        {
            if(!_context.Products.Any(x => x.ProductId == productId))
                throw new Exception("Product not found");

            // get offers that have the product and not expired  then include the productoffer 

            var _offers = await _context.Offers.Where(x => x.ProductOffers.Any(x => x.ProductId == productId)).ToListAsync();
            _offers = _offers.Where(x => !OfferExpiredOrInActive(x)).ToList();

            return _offers;
           
        }

        public async Task<int> AddOffer(OfferDTO offerDTO)
        {
            try
            {

                var offer = _mapper.Map<Offer>(offerDTO);
                offer.Image = await UploadImages(offerDTO.Image);
                offer.PackageDiscount = offerDTO.PackageDiscount ?? 0;
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

        public async Task<OffersDTOUI> GetOfferById(int id)
        {
            try
            {
                var offer = await _context.Offers.Include(x => x.ProductOffers).FirstOrDefaultAsync(x => x.OfferId == id);
                if (offer == null)
                    throw new Exception("Offer not found");
                if (OfferExpiredOrInActive(offer))
                    throw new Exception("Offer is expired or inactive");

                var offerDTO = _mapper.Map<OffersDTOUI>(offer);
                offerDTO.ProductOffers = _mapper.Map<List<OfferProductsDTO>>(offer.ProductOffers);


                return offerDTO;
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
                if (PackageDiscount != null)
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


        public async Task UpdateOffer(OffersDTOUI offersDTOUI)
        {
            try
            {
                var offer = await _context.Offers.Include(x => x.ProductOffers).FirstOrDefaultAsync(x => x.OfferId == offersDTOUI.OfferId);
                if (offer == null)
                    throw new Exception("Offer not found");
                //update the offerProducts
                _context.ProductOffers.RemoveRange(offer.ProductOffers);
                var offerProducts = _mapper.Map<List<ProductOffer>>(offersDTOUI.ProductOffers);
    

                foreach (var item in offerProducts)
                {
                    item.OfferId = offersDTOUI.OfferId;
                    offer.ProductOffers.Add(item);
                }


                //update the offer
                _mapper.Map(offersDTOUI, offer);

 
               
                await _context.SaveChangesAsync();
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
                //remove the offer
                _context.Offers.Remove(offer);

                // remove image from cloudinary
                var publicId = offer.Image.Split("/").Last().Split(".")[0];
                await _fileCloudService.DeleteImage(publicId);


                //remove the products assigned to the offer
                var offerProducts = await _context.ProductOffers.Where(x => x.OfferId == offerId).ToListAsync();
                _context.ProductOffers.RemoveRange(offerProducts);



                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
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

            var uploadResult = await _fileCloudService.UploadImages(picture);

            // Handle the result as needed, e.g., save the image URL to your database
            return uploadResult;

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
                return false;

            return true;
        }
        public  bool OfferExpiredOrInActive(Offer offer)
        {
            
            // Check if the offer is null
            if (offer == null)
                return true;

            // Calculate the end date of the offer
            var startDate = offer.OfferDate;
            int durationInDays = offer.Duration ?? 0; // Assuming a duration of 0 if it is null

            if (DateTime.Now > startDate.ToDateTime(TimeOnly.MinValue).AddDays(durationInDays))
                return false;

            return true;
        }



    }
}
