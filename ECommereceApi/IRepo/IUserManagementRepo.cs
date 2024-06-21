using ECommereceApi.Data;
using ECommereceApi.DTOs.Account;
using ECommereceApi.Models;

namespace ECommereceApi.IRepo;

public interface IUserManagementRepo
{
    Task<User?> GetUserByEmail(string email);

    Task<bool> TryRegisterUser(RegisterUserDTO dto,string code);

    Task<bool> TryChangeVerificationCode(string email, string code);

    Task<bool> TryAlterUserData(AlterUserDataDTO dto);

    Task<bool> TryResetPassword(string email, string password);

    Task<bool> ConfirmEmail(VerifyEmail verifyModel);

}
