using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresResourceManipulation;

public class UpdateCommentDTO
{
    [Required]
    public string Content { get; set; }
}
