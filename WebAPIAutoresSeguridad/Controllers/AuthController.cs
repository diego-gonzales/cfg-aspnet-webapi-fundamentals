using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebAPIAutoresSeguridad;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly IConfiguration configuration;

    // UserManager es un servicio que nos permite administrar los usuarios. Debemos pasarle una clase que identifica a un usuario en nuestra app, en nuestro caso estamos usando la clase por defecto 'IdentityUser'.
    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        this.userManager = userManager;
        this.configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDTO>> Register(RegisterDTO registerDTO)
    {
        var user = new IdentityUser() { UserName = registerDTO.Email, Email = registerDTO.Email };
        var result = await userManager.CreateAsync(user, registerDTO.Password);

        if (result.Succeeded)
        {
            return GetToken(registerDTO);
        }

        return BadRequest(result.Errors);
    }

    private AuthResponseDTO GetToken(RegisterDTO registerDTO)
    {
        // Un Claim es información confiable acerca del usuario que se va a guardar en el token. En este caso, el Claim que vamos a guardar es el email del usuario. Se podría decir que es el payload del token.
        var claims = new List<Claim>()
        {
            new Claim("email", registerDTO.Email),
            // new Claim("lo que yo quiera", "123")
        };

        var jwtSecret = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["JWTSecret"])
        );

        var creds = new SigningCredentials(jwtSecret, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddDays(1);

        var securityToken = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new AuthResponseDTO()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
            Expiration = expiration
        };
    }
}
