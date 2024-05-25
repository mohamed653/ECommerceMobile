using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Account;

public record LoginUserDTO
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Password { get; set; }

}
