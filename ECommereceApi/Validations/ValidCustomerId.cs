using ECommereceApi.DTOs.Account;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.Validations;

public class ValidCustomerId : ValidationAttribute
{

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return new ValidationResult("customer Id is required!");

        int? customerId = value as int?;
        if (customerId is null)
            return new ValidationResult("customer Id should be integer value!");

        IUserRepo userRepo = (IUserRepo) validationContext.GetService(typeof(IUserRepo));

        bool isValidCustomerId = userRepo.UserExistsAsync(customerId.Value).Result;

        if (!isValidCustomerId)
            return new ValidationResult("Customer Id is incorrect!");



        return ValidationResult.Success;
    }
}
