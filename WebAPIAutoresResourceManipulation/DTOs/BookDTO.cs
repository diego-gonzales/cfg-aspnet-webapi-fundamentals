namespace WebAPIAutoresResourceManipulation;

public class BookDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<CommentDTO> Comments { get; set; }
    public List<AuthorDTO> Autores { get; set; }
}
