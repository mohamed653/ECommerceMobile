using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Account
{
    public class ResetPasswordDTO
    {
        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string? NewPassword { get; set; }

        [Required]
        [MaxLength(50)]
        [Compare(nameof(NewPassword))]
        public string? ConfirmNewPassword { get; set; }
    }
}
