namespace WebAPIAutoresAvanzados;

public class AuthorWithBooksDTO : AuthorDTO
{
    public List<BookDTO> Books { get; set; }
}
