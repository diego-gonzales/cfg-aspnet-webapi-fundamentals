using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresResourceManipulation;

public class Libro
{
    public int Id { get; set; }

    [Required]
    [StringLength(
        maximumLength: 250,
        ErrorMessage = "El campo {0} no debe tener más de {1} caracteres"
    )]
    [UpperFirstLetter]
    public string Name { get; set; }
}
