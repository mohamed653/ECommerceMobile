using ECommereceApi.Data;
using ECommereceApi.DTOs.Account;
using ECommereceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class UserManagementRepo : IUserManagementRepo
    {
        private readonly ECommerceContext _context;

        public UserManagementRepo(ECommerceContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            User? user =await _context.Users
                                .FirstOrDefaultAsync(user => user.Email.Equals(email));

            return user;
        }

        public async Task<bool> TryAlterUserData(AlterUserDataDTO dto)
        {
            User? user =await GetUserByEmail(dto.Email);

            if (user is null)
                return false;

            user.FName = dto.FName;
            user.LName = dto.LName;
            user.Street = dto.Email;
            user.City = dto.City;
            user.Phone = dto.Phone; 
            user.PostalCode = dto.PostalCode;
            user.Governorate = dto.Governorate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TryRegisterUser(RegisterUserDTO dto)
        {
            bool emailExisted =await _context.Users
                                        .AnyAsync(user => user.Email.Equals(dto.Email));
            if (emailExisted)
            {
                return false;
            }

            User user = new User()
            {
                FName = dto.FName,
                LName = dto.LName,
                Email = dto.Email,
                Phone = dto.phone,
                Password = dto.Password,
                Role = RoleType.Customer
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TryResetPassword(string email, string newPassword)
        {
            User user = await GetUserByEmail(email);
            if(user is null)
                return false;

            user.Password = newPassword;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
