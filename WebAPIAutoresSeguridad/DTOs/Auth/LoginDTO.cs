using System.ComponentModel.DataAnnotations;

namespace WebAPIAutoresSeguridad;

public class LoginDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    // Identity por defecto configura las otras reglas de validación como: al menos 6 caracteres, una mayúscula y una minúscula.
    [Required]
    public string Password { get; set; }
}
