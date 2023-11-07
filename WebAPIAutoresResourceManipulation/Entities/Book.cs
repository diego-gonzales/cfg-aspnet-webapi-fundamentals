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

    // this is only the 'navigation properties', which allows us to easily perform JOINs. It's only used if we want in the querys.
    public List<Comment> Comments { get; set; }

    // al recibir en mi CreateBookDTO un listado de IDs de autores entonces en mi 'AutoMapperProfiles' debo tomar en cuenta esta propiedad y mapear esos IDs de autores en una lista de 'AuthorsBooks' (🚩🚩🌍🌍).
    public List<AuthorBook> AuthorsBooks { get; set; }
}
