namespace WebAPIAutoresResourceManipulation;

// se crea una clase intermedia para una relación de muchos a muchos, en este ejemplo lo hay entre autores y libros.
// recuerda configurar la llave primaria compuesta de nuestra entidad AuthorBook, que seria la composición de los campos AuthorId y BookId, esto se configura usando el 'API FLUENTE' de EFCore, este se configura dentro del 'ApplicationDbContext' (👀👀🙈🙈).
public class AuthorBook
{
    public int Order { get; set; }
    public int AuthorId { get; set; }
    public int BookId { get; set; }

    // 'navigation properties'
    public Author Author { get; set; }
    public Book Book { get; set; }
}
