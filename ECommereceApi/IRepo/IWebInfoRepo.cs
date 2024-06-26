using ECommereceApi.DTOs.WebInfo;
using ECommereceApi.Models;

namespace ECommereceApi.IRepo
{
    public interface IWebInfoRepo
    {
        Task<WebInfo> GetWebInfo();
        Task UpdateWebInfo(WebInfoDTO webInfo);
        Task AddWebInfo(WebInfoDTO webInfo);
    }
}
