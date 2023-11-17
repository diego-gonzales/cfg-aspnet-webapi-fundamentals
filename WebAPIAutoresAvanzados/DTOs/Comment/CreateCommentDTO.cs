using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresAvanzados;

public class CreateCommentDTO
{
    [Required]
    public string Content { get; set; }
}
