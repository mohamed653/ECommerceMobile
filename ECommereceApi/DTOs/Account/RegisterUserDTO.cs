using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommereceApi.DTOs.Account;

public record RegisterUserDTO
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string? FName { get; set; }

    [StringLength(50, MinimumLength = 3)]
    public string? LName { get; set; }

    [Required]
    [StringLength(50)]
    [EmailAddress]
    public string? Email { get; set; }


    [Required]
    [RegularExpression(@"^(010|011|012|015)\d{8}$")]
    [Display(Name = "Phone Number")]
    public string? phone { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 8)]
    public string? Password { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 8)]
    public string? ConfirmPassword { get; set; }

}
