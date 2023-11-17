using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresAvanzados;

public class UpdateCommentDTO
{
    [Required]
    public string Content { get; set; }
}
