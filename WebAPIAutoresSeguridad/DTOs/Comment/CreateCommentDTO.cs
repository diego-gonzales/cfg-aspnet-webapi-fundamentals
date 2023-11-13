using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresSeguridad;

public class CreateCommentDTO
{
    [Required]
    public string Content { get; set; }
}
