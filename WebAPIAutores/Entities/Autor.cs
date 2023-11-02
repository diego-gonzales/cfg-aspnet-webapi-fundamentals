using System.ComponentModel.DataAnnotations;

namespace WebAPIAutores;

public class Autor : IValidatableObject // interface para hacer validaciones a nivel de modelo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo '{0}' es requerido")]
    [StringLength(
        maximumLength: 100,
        ErrorMessage = "El campo '{0}' no debe tener más de {1} caracteres"
    )]
    // [UpperFirstLetter]
    public string Name { get; set; }

    // propiedad de navegación con la que voy a poder cargar los libros de un autor
    public List<Libro> Libros { get; set; }

    // [NotMapped] // nos permite tener propiedades en nuestras entidades que no se van a corresponder con una columna de la tabla
    // [Range(18, 50)]
    // public int Edad { get; set; }

    // [NotMapped]
    // [CreditCard]
    // public string TarjetaCredito { get; set; }

    // [NotMapped]
    // [Url]
    // public string URL { get; set; }

    // [NotMapped]
    // public int Menor { get; set; }

    // [NotMapped]
    // public int Mayor { get; set; }

    // Validaciones por modelo
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Name))
        {
            var firstLetter = Name[0].ToString();

            if (firstLetter != firstLetter.ToUpper())
            {
                yield return new ValidationResult(
                    $"La primera letra del campo {nameof(Name)} debe ser mayúscula",
                    new string[] { nameof(Name) }
                );
            }
        }

        // if (Menor > Mayor)
        // {
        //   yield return new ValidationResult($"El campo {nameof(Menor)} no puede ser mayor que el campo {nameof(Mayor)}", new string[] { nameof(Menor) });
        // }
    }
}
