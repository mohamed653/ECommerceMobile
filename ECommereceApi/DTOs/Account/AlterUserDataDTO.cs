using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Account;

public class AlterUserDataDTO
{

    [Required]
    [StringLength(50)]
    public string Email { get; set; }

    [Required]
    [StringLength(50)]
    public string FName { get; set; }

    [StringLength(50)]
    public string? LName { get; set; }

    [Required]
    [StringLength(50)]
    [RegularExpression(@"^(010|011|012|015)\d{8}$")]
    public string? Phone { get; set; }

    [StringLength(50)]
    public string? Governorate { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(50)]
    public string? Street { get; set; }

    [StringLength(50)]
    public string? PostalCode { get; set; }

}
