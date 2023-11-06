namespace WebAPIAutoresResourceManipulation;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int BookId { get; set; }

    // this is only a 'navigation property', which allows us to easily perform JOINs. It's only used if we want.
    public Book Book { get; set; }
}
