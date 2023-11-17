using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresAvanzados;

public class BecomeAdminDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
