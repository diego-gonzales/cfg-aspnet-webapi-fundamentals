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
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly IConfiguration configuration;

    // UserManager es un servicio que nos permite administrar los usuarios. Debemos pasarle una clase que identifica a un usuario en nuestra app, en nuestro caso la clase por defecto 'IdentityUser' representa un usuario.
    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration
    )
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
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

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO loginDTO)
    {
        var result = await signInManager.PasswordSignInAsync(
            loginDTO.Email,
            loginDTO.Password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            return GetToken(loginDTO);
        }

        return BadRequest("Login incorrecto. Credenciales son incorrectas");
    }

    private AuthResponseDTO GetToken(LoginDTO loginDTO)
    {
        // Un Claim es información confiable acerca del usuario que se va a añadir en el token. En este caso, el Claim que vamos a guardar es el email del usuario. Se podría decir que es el payload del token.
        var claims = new List<Claim>()
        {
            new Claim("email", loginDTO.Email),
            new Claim("lo que yo quiera", "cualquier valor")
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
            // el tiempo de expiración esta de manera implícita dentro del token, pero aquí de igual manera la pasamos explí/.
            Expiration = expiration
        };
    }
}
