using System;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.Validations
{
    public class ValidReviewDate : ValidationAttribute
    {
      
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
                return new ValidationResult("Review Date is required!");

            DateOnly? reviewDate = value as DateOnly?;
            if (reviewDate is null)
                return new ValidationResult("Invalid Date Format!!");

            bool isValidReviewDate = reviewDate <= DateOnly.FromDateTime(DateTime.Now) ;
            if (!isValidReviewDate)
                return new ValidationResult("Review Date can't be incoming date!");

            return ValidationResult.Success;
        }
    }
}
