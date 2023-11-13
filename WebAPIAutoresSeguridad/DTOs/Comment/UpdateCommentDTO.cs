using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresSeguridad;

public class UpdateCommentDTO
{
    [Required]
    public string Content { get; set; }
}
