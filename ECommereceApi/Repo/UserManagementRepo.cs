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

        public async Task<bool> ConfirmEmail(VerifyEmail verifyModel)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(user => user.Email == verifyModel.Email);
                if (user is null)
                    return false;

                if (user.VertificationCode != verifyModel.Code)
                    return false;

                user.VerifiedAt = DateTime.UtcNow;
                user.IsVerified = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
            
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

        public async Task<bool> TryRegisterUser(RegisterUserDTO dto, string code)
        {
            try
            {
                bool emailExisted = await _context.Users
                                        .AnyAsync(user => user.Email.Equals(dto.Email));
                if (emailExisted)
                {
                    return false;
                }

                //code = GenerateCode().ToString();
                User user = new User()
                {
                    FName = dto.FName,
                    LName = dto.LName,
                    Email = dto.Email,
                    Phone = dto.phone,
                    Password = dto.Password,
                    Role = RoleType.Customer,
                    VertificationCode = code,
                    IsVerified = false,
                    IsDeleted = false
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return true;
            }catch
            {
                return false;
            }
            
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
