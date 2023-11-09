namespace WebAPIAutoresResourceManipulation;

public class BookWithCommentsAndAuthorsDTO : BookDTO
{
    public List<CommentDTO> Comments { get; set; }
    public List<AuthorDTO> Autores { get; set; }
}
