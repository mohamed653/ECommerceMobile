using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.WebInfo
{
    public class WebInfoDTO
    {

        [StringLength(50)]
        [RegularExpression(@"^01[0-2]{1}[0-9]{8}$", ErrorMessage = "Invalid Phone Number")]
        [Required]
        public string WebPhone { get; set; }

        [Required]
        [StringLength(50)]
        public string WebName { get; set; }

        public IFormFile WebLogo { get; set; }

        [StringLength(50)]
        //[RegularExpression(@"^@[a-zA-Z0-9_.]+$", ErrorMessage = "Invalid Account")]
        public string InstagramAccount { get; set; }

        [StringLength(50)]
        //[RegularExpression(@"^@[a-zA-Z0-9_.]+$", ErrorMessage = "Invalid Account")]
        public string FacebookAccount { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? StoreAddress { get; set; }

        [Required]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone number is not valid")]
        public string CustomerServicePhone { get; set; }

    }
}