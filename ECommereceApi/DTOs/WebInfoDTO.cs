using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs
{
    public class WebInfoDTO
    {

        [StringLength(50)]
        public string WebPhone { get; set; }

        [StringLength(50)]
        public string WebName { get; set; }

        public IFormFile WebLogo { get; set; }

        [StringLength(50)]
        public string InstagramAccount { get; set; }

        [StringLength(50)]
        public string FacebookAccount { get; set; }
    }
}
