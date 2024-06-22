using ECommereceApi.Models;

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
        public bool IsDeleted { get; set; }
        public RoleType Role { get; set; }
    }
    public class UserDTOUi
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Governorate { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; } = false;
        public RoleType Role { get; set; }
    }

}
