using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresSeguridad;

public class Author
{
    public int Id { get; set; }

    [Required]
    [StringLength(
        maximumLength: 150,
        ErrorMessage = "El campo '{0}' no debe tener más de {1} caracteres"
    )]
    [UpperFirstLetter]
    public string Name { get; set; }

    // 'navigation property'
    public List<AuthorBook> AuthorsBooks { get; set; }
}
