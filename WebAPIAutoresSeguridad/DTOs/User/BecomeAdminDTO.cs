using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresSeguridad;

public class BecomeAdminDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
