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
                user.VertificationCode = "";
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
            user.Email = dto.Email.ToLower().Trim();
            user.Street = dto.Street;
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

                User user = new User()
                {
                    FName = dto.FName,
                    LName = dto.LName,
                    Email = dto.Email.ToLower().Trim(),
                    Phone = dto.phone,
                    Password = dto.Password,
                    Role = RoleType.Customer,
                    VertificationCode = code,
                    IsVerified = false,
                    IsDeleted = false
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                await  _context.Customers.AddAsync(new Customer { UserId = user.UserId });
                await _context.SaveChangesAsync();
                return true;
            }catch
            {
                return false;
            }
            
        }

        public async Task<bool> TryChangeVerificationCode(string email, string code)
        {
            User? user = await GetUserByEmail(email);
            if(user is null) return false;

            user.VertificationCode = code;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TryResetPassword(string email,string password)
        {
            User? user = await GetUserByEmail(email);
            if (user is null) return false;

            user.Password = password;
            user.VertificationCode= string.Empty;

            await _context.SaveChangesAsync();
            return true;
        }


    }


}
