using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresResourceManipulation;

public class CreateCommentDTO
{
    [Required]
    public string Content { get; set; }
}
