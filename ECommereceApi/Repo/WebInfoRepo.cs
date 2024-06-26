
using AutoMapper;
using ECommereceApi.Data;
using ECommereceApi.DTOs.WebInfo;
using ECommereceApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class WebInfoRepo : IWebInfoRepo
    {
        private readonly ECommerceContext _context;
        private readonly IMapper _mapper;
        private readonly IFileCloudService _fileCloudService;
        private readonly IConfiguration _configuration;
        public WebInfoRepo(ECommerceContext context, IMapper mapper, IFileCloudService fileCloudService, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _fileCloudService = fileCloudService;
            _configuration = configuration;
        }
        public async Task AddWebInfo(WebInfoDTO webInfoDTO)
        {
            if (webInfoDTO == null)
                throw new Exception("Web info is null");
            if (_context.Web_Infos.Any())
                throw new Exception("Web info already exist");
            var publicId = await _fileCloudService.UploadImagesAsync(webInfoDTO.WebLogo);

            var webInfo = _mapper.Map<WebInfo>(webInfoDTO);
            webInfo.WebLogoImageUrl = publicId;
            await _context.Web_Infos.AddAsync(webInfo);
            await _context.SaveChangesAsync();

        }

        public async Task<WebInfo> GetWebInfo()
        {
            var webInfo = await _context.Web_Infos.FirstOrDefaultAsync();
            if (webInfo == null)
            {
                // add new web info with default values  get the values from appsettings.json

                var config = _configuration.GetSection("defaultWebInfo").Get<WebInfo>();

                if (config == null)
                    throw new Exception("Web info not found in the database and no default values found in appsettings.json");

                var newWebInfo = new WebInfo
                {
                    WebPhone = config.WebPhone,
                    WebLogoImageUrl =config.WebLogoImageUrl,
                    WebName= config.WebName,
                    InstagramAccount = config.InstagramAccount,
                    FacebookAccount = config.FacebookAccount,
                };

                _context.Web_Infos.Add(newWebInfo);
                await _context.SaveChangesAsync();
                
                var _webInfo = await _context.Web_Infos.FirstOrDefaultAsync();
                return _webInfo;
            }
            webInfo.WebLogoImageUrl = _fileCloudService.GetImageUrl(webInfo.WebLogoImageUrl);

            return webInfo;
        }

        public async Task UpdateWebInfo(WebInfoDTO webInfoDTO)
        {
            if (webInfoDTO == null)
            {
                throw new ArgumentNullException(nameof(webInfoDTO), "WebInfoDTO cannot be null");
            }

            var oldWebInfo = await _context.Web_Infos.FirstOrDefaultAsync();

            if (oldWebInfo == null)
            {
                throw new Exception("Web info not found");
            }

            if (webInfoDTO.WebLogo != null)
            {
                var newPublicId = await _fileCloudService.UpdateImageAsync(webInfoDTO.WebLogo, oldWebInfo.WebLogoImageUrl);

                if (newPublicId != null)
                {
                    oldWebInfo.WebLogoImageUrl = newPublicId;
                }
                else
                {
                    throw new Exception("Failed to update web logo");
                }
            }

            oldWebInfo.WebName = webInfoDTO.WebName ?? oldWebInfo.WebName;
            oldWebInfo.WebPhone = webInfoDTO.WebPhone ?? oldWebInfo.WebPhone;
            oldWebInfo.InstagramAccount = webInfoDTO.InstagramAccount ?? oldWebInfo.InstagramAccount;
            oldWebInfo.FacebookAccount = webInfoDTO.FacebookAccount ?? oldWebInfo.FacebookAccount;

            _context.Web_Infos.Update(oldWebInfo);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IWebInfoExist()
        {
            return await _context.Web_Infos.AnyAsync();
        }
    }
}
