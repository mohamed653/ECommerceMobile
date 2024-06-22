using AutoMapper;
using CloudinaryDotNet;
using ECommereceApi.DTOs.Product;
using ECommereceApi.Services.classes;
using ECommereceApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{

    public class OrderRepo : IOrderRepo
    {
        private readonly ECommerceContext _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IFileCloudService _fileCloudService;
        public OrderRepo(IWebHostEnvironment env, ECommerceContext db, IMapper mapper, IFileCloudService fileCloudService)
        {
            _db = db;
            _mapper = mapper;
            _env = env;
            _fileCloudService = fileCloudService;
        }
        public async Task<OrderDisplayDTO> GetOrderAsync(int userId)
        {
            var result = _db.ProductCarts.FirstOrDefault(pc => pc.UserId == userId);
            if (result == null)
            {
                return null;
            }
            var order = _mapper.Map<OrderDisplayDTO>(result);
            return order;
        }


    }
}
