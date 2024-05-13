using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Account;

public class VerifyEmail
{
    [Required]
    [StringLength(50)]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(6)]
    public string Code { get; set; }
}
