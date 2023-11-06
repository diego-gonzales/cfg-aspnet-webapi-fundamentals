using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresResourceManipulation;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(
        maximumLength: 250,
        ErrorMessage = "El campo {0} no debe tener más de {1} caracteres"
    )]
    [UpperFirstLetter]
    public string Name { get; set; }

    // this is only a 'navigation property', which allows us to easily perform JOINs. It's only used if we want.
    public List<Comment> Comments { get; set; }
}
