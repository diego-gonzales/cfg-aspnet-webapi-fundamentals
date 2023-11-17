using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresAvanzados;

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
    public DateTime? PublicationDate { get; set; }

    public List<Comment> Comments { get; set; }
    public List<AuthorBook> AuthorsBooks { get; set; }
}
