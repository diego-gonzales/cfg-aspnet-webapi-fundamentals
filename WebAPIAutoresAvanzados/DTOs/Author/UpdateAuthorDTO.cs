using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresAvanzados;

public class UpdateAuthorDTO
{
    [Required]
    [StringLength(
        maximumLength: 150,
        ErrorMessage = "El campo '{0}' no debe tener más de {1} caracteres"
    )]
    [UpperFirstLetter]
    public string Name { get; set; }
}
