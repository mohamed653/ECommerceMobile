using ECommereceApi.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Account
{
    public class UserDTO
    {

        public int UserId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Governorate { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
        public RoleType Role { get; set; }
    }
    public class UserDTOUi
    {
        [StringLength(50)]
        public string FName { get; set; }
        [StringLength(50)]
        public string LName { get; set; }
        // email validation
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string Phone { get; set; }
        [StringLength(50)]
        public string? Governorate { get; set; }
        [StringLength(50)]
        public string? City { get; set; }
        [StringLength(50)]
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        [MinLength(8)]
        public string Password { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
        public RoleType Role { get; set; }
    }

    public class UserUpdateDTO
    {
        [StringLength(50)]
        public string FName { get; set; }
        [StringLength(50)]
        public string LName { get; set; }
        // Note: Changing Email needs verification
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string Phone { get; set; }
        [StringLength(50)]
        public string? Governorate { get; set; }
        [StringLength(50)]
        public string? City { get; set; }
        [StringLength(50)]
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
    }

}
