using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Account;

public class forgotPasswordDTO
{
    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; }

    [Required]
    [StringLength(6)]
    public string Code{ get; set; }


    [Required]
    [StringLength(50,MinimumLength =8)]
    public string? NewPassword { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 8)]
    [Compare(nameof(NewPassword))]
    public string? ConfirmNewPassword { get; set; }

}
