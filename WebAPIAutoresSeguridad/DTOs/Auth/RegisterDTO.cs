using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresSeguridad;

public class RegisterDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 6)]
    public string Password { get; set; }
}
