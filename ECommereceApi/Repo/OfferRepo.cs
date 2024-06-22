using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Services.Interfaces;
using MethodTimer;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ECommereceApi.Repo
{
    public class OfferRepo : IOfferRepo
    {
        #region Fields
        private readonly ECommerceContext _context;
        private readonly IFileCloudService _fileCloudService;
        private readonly IMapper _mapper;
        private readonly IProductRepo _productRepo;
        #endregion
        #region Constructors
        public OfferRepo(ECommerceContext context, IFileCloudService fileCloudService, IMapper mapper, IProductRepo productRepo)
        {
            _context = context;
            _fileCloudService = fileCloudService;
            _mapper = mapper;
            _productRepo = productRepo;
        }
        #endregion

        #region Methods

        [Time] //Testing the performance of the method using the MethodTimer library
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
                    offer.ProductOffers = _mapper.Map<List<OfferProductsDetailedDTO>>(offers.FirstOrDefault(x => x.OfferId == offer.OfferId).ProductOffers);
                    await LoadProductDetails(offer);
                }
                return offersDTO;
            }
            catch (Exception)
            {

                Log.Error($"Error in GetOffersWithProducts");
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
                Log.Error($"Error in AddOffer");
                throw;
            }
        }

        //public async Task<OffersDTOUI> GetOfferById(int id)
        //{
        //    try
        //    {
        //        var offer = await _context.Offers.Include(x => x.ProductOffers).FirstOrDefaultAsync(x => x.OfferId == id);
        //        if (offer == null)
        //            throw new Exception("Offer not found");
        //        if (OfferExpiredOrInActive(offer))
        //            throw new Exception("Offer is expired or inactive");

        //        var offerDTO = _mapper.Map<OffersDTOUI>(offer);
        //        offerDTO.ProductOffers = _mapper.Map<List<OfferProductsDTO>>(offer.ProductOffers);


        //        return offerDTO;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}

 
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
                offerDTO.ProductOffers = _mapper.Map<List<OfferProductsDetailedDTO>>(offer.ProductOffers);
                await LoadProductDetails(offerDTO);

                return offerDTO;
            }
            catch (Exception)
            {
                Log.Error($"Error in GetOfferById");
                throw;
            }

        }

        private async Task LoadProductDetails(OffersDTOUI offersDTOUI)
        {
            foreach (var item in offersDTOUI.ProductOffers)
            {
                var product = await _context.Products.Include(p=>p.ProductImages).FirstOrDefaultAsync(p=>p.ProductId==item.ProductId);
                if (product == null)
                    throw new Exception("Product not found");
                item.Name = product.Name;
                if(product.ProductImages.Count>0)
                    item.Image = product.ProductImages.FirstOrDefault().ImageId;
            }
        }   
        public async Task AddProductsToOffer(int offerId, OffersDTOPost offerProductsDTO)
        {
            try
            {
               
                var offer = await _context.Offers.Include(x => x.ProductOffers).FirstOrDefaultAsync(x => x.OfferId == offerId);
                if (offer == null)
                    throw new Exception("Offer not found");
                
                var product = await _context.Products.FindAsync(offerProductsDTO.ProductId);
                if (product == null)
                    throw new Exception("Product not found");

                if (offer.ProductOffers.Any(x => x.ProductId == offerProductsDTO.ProductId))
                    throw new Exception("Product already exists in the offer");

                var productOffer = new ProductOffer()
                {
                    OfferId = offerId,
                    ProductId = offerProductsDTO.ProductId,
                    ProductAmount = offerProductsDTO.ProductAmount,
                    Discount = offerProductsDTO.Discount
                };

                offer.ProductOffers.Add(productOffer);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                Log.Error($"Error in AddProductsToOffer");
                throw;
            }
        }


        public async Task UpdateOffer(int offerId, OfferDTO offerDTO)
        {
            try
            {
                var offer = await _context.Offers.FindAsync(offerId);
                if (offer == null)
                    throw new Exception("Offer not found");

                offer.Title = offerDTO.Title;
                offer.Description = offerDTO.Description;
                offer.OfferDate = offerDTO.OfferDate;
                offer.Duration = offerDTO.Duration;
                offer.PackageDiscount = offerDTO.PackageDiscount;
                if (offerDTO.Image != null)
                {
                    // delete the old image from cloudinary
                    var publicId = offer.Image.Split("/").Last().Split(".")[0];
                    await _fileCloudService.DeleteImageAsync(publicId);

                    // upload the new image
                    offer.Image = await UploadImages(offerDTO.Image);
                   
                }

                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Status> UpdateProductsFromOffer(int offerId, int oldProductId, OffersDTOPost offerProductsDTO)
        {
            try
            {
                var offer = await _context.Offers.Include(x => x.ProductOffers).FirstOrDefaultAsync(x => x.OfferId == offerId);

                if (offer == null)
                    throw new Exception("Offer not found");

                var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == oldProductId); 

                if (product == null)
                    throw new Exception("Product not found");

                // check if the product is already in the offer
                if (!offer.ProductOffers.Any(x => x.ProductId == oldProductId))
                    throw new Exception("Product doesn't exist in the offer");

                // remove the productOffer and add it again with the new values

                var productOffer = offer.ProductOffers.FirstOrDefault(x => x.ProductId == oldProductId);

                // Remove the productOffer from the Offer's collection
                offer.ProductOffers.Remove(productOffer);

                // Create a new ProductOffer with the updated values
                var newProductOffer = new ProductOffer
                {
                    OfferId = offerId,
                    ProductId = offerProductsDTO.ProductId,
                    ProductAmount = offerProductsDTO.ProductAmount,
                    Discount = offerProductsDTO.Discount
                };

                // Add the new ProductOffer to the Offer's collection
                offer.ProductOffers.Add(newProductOffer);

                // Save changes to the context
                await _context.SaveChangesAsync();
                return Status.Success;
            }
            catch (Exception)
            {
                Log.Error($"Error in UpdateProductsFromOffer");
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
                await _fileCloudService.DeleteImageAsync(publicId);


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

        public async Task<List<OffersDTOUI>> RemoveProductFromOffer(int offerId, int productId)
        {
            try
            {
                var offer = await _context.Offers.Include(x => x.ProductOffers).FirstOrDefaultAsync(x => x.OfferId == offerId);
                if (offer == null)
                    throw new Exception("Offer not found");

                var productOffer = offer.ProductOffers.FirstOrDefault(x => x.ProductId == productId);
                if (productOffer == null)
                    throw new Exception("Product not found in the offer");

                offer.ProductOffers.Remove(productOffer);
                await _context.SaveChangesAsync();

                return await GetOffersWithProducts();
            }
            catch (Exception)
            {
                Log.Error($"Error in RemoveProductFromOffer");
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

            var uploadResult = await _fileCloudService.UploadImagesAsync(picture);

            // Handle the result as needed, e.g., save the image URL to your database
            return _fileCloudService.GetImageUrl(uploadResult);

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
            var _date = startDate.ToDateTime(TimeOnly.MinValue).AddDays(durationInDays);
            if (DateTime.Now < _date)
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

            var _date = startDate.ToDateTime(TimeOnly.MinValue).AddDays(durationInDays);
            if (DateTime.Now < _date )
                return false;

            return true;
        }

        #endregion


    }
}
