using Microsoft.AspNetCore.Identity;

namespace WebAPIAutoresAvanzados;

public class CommentWithBookDTO : CommentDTO
{
    public BookDTO Book { get; set; }
    public IdentityUserDTO User { get; set; }
}
