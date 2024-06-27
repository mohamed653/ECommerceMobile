using ECommereceApi.DTOs.Product;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.Validations;

public class ValidProductId : ValidationAttribute
{
   
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult("product id is required!");

        int? productId = value as int?;
        if (productId is null)
            return new ValidationResult("product id should be integer!");

        IProductRepo productRepo = (IProductRepo)validationContext.GetService(typeof(IProductRepo));

        ProductDisplayDTO ExistingProduct = productRepo.GetProductByIdAsync(productId.Value).Result;
        if (ExistingProduct is null)
            return new ValidationResult("No product with this id!");

        return ValidationResult.Success;
    }
}
