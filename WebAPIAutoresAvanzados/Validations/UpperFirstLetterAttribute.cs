using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresAvanzados;

public class UpperFirstLetterAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            return ValidationResult.Success;
        }

        var firstLetter = value.ToString()[0].ToString();

        if (firstLetter != firstLetter.ToUpper())
        {
            return new ValidationResult(
                $"La primera letra del campo {validationContext.MemberName} debe ser mayúscula"
            );
        }

        return ValidationResult.Success;
    }
}
