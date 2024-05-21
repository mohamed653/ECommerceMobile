using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs
{
    public class WebInfoDTO
    {

        [StringLength(50)]
        public string WebPhone { get; set; }

        [StringLength(50)]
        public string WebName { get; set; }

        [StringLength(200)]
        public string WebLogoImageUrl { get; set; }

        [StringLength(50)]
        public string InstagramAccount { get; set; }

        [StringLength(50)]
        public string FacebookAccount { get; set; }
    }
}
