using System.ComponentModel.DataAnnotations;

namespace WebAPIAutores;
public class UpperFirstLetterAttribute : ValidationAttribute // interface para hacer validaciones a nivel de atributo
{
  protected override ValidationResult IsValid(object value, ValidationContext validationContext)
  {
    // Si es 'null' retornamos una validación exitosa ya que no queremos solapar la validación de 'Required'
    if (value == null || string.IsNullOrEmpty(value.ToString()))
    {
      return ValidationResult.Success;
    }

    var firstLetter = value.ToString()[0].ToString();

    if (firstLetter != firstLetter.ToUpper())
    {
      return new ValidationResult($"La primera letra del campo {validationContext.MemberName} debe ser mayúscula");
    }

    return ValidationResult.Success;
  }
}
