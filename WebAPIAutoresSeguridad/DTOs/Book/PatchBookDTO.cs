using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresSeguridad;

public class PatchBookDTO
{
    [StringLength(
        maximumLength: 250,
        ErrorMessage = "El campo {0} no debe tener más de {1} caracteres"
    )]
    [UpperFirstLetter]
    public string Name { get; set; }
    public DateTime PublicationDate { get; set; }
}
