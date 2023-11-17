using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebAPIAutoresAvanzados;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly IConfiguration configuration;
    private readonly IDataProtector dataProtector;

    // 'UserManager' es un servicio que nos permite administrar los usuarios. Debemos pasarle una clase que identifica a un usuario en nuestra app, en nuestro caso la clase por defecto 'IdentityUser' representa un usuario.
    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        IDataProtectionProvider dataProtectionProvider // servicio para realizar la encriptación de datos
    )
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.configuration = configuration;
        dataProtector = dataProtectionProvider.CreateProtector("unique_value_and_secret"); // debería venir de env variables
    }

    [HttpPost("register", Name = "registerUser")]
    public async Task<ActionResult<AuthResponseDTO>> Register(RegisterDTO registerDTO)
    {
        var user = new IdentityUser() { UserName = registerDTO.Email, Email = registerDTO.Email };
        var result = await userManager.CreateAsync(user, registerDTO.Password);

        if (result.Succeeded)
        {
            return await GetToken(registerDTO);
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login", Name = "loginUser")]
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
            return await GetToken(loginDTO);
        }

        return BadRequest("Login incorrecto. Credenciales son incorrectas");
    }

    [HttpGet("refresh-token", Name = "refreshToken")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<AuthResponseDTO>> Refresh()
    {
        var emailClaim = HttpContext.User.Claims
            .Where(claim => claim.Type == "email")
            .FirstOrDefault();
        var email = emailClaim.Value;

        var loginDto = new LoginDTO() { Email = email };
        return await GetToken(loginDto);
    }

    [HttpPost("become-admin", Name = "becomeAdmin")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public async Task<ActionResult> BecomeAdmin(BecomeAdminDTO becomeAdminDTO)
    {
        var user = await userManager.FindByEmailAsync(becomeAdminDTO.Email);
        await userManager.AddClaimAsync(user, new Claim("isAdmin", true.ToString()));
        return NoContent();
    }

    [HttpPost("remove-admin", Name = "removeAdmin")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public async Task<ActionResult> RemoveAdmin(BecomeAdminDTO becomeAdminDTO)
    {
        var user = await userManager.FindByEmailAsync(becomeAdminDTO.Email);
        await userManager.RemoveClaimAsync(user, new Claim("isAdmin", true.ToString()));
        return NoContent();
    }

    private async Task<AuthResponseDTO> GetToken(LoginDTO loginDTO)
    {
        // Un Claim es información confiable acerca del usuario que se va a añadir en el token. En este caso, el Claim que vamos a guardar es el email del usuario. Se podría decir que es el payload del token.
        var claims = new List<Claim>()
        {
            new Claim("email", loginDTO.Email),
            new Claim("lo que yo quiera", "cualquier valor")
        };

        var user = await userManager.FindByEmailAsync(loginDTO.Email);
        var claimsDB = await userManager.GetClaimsAsync(user);
        // Unimos los claims que vienen de la base de datos (en este caso, el claim 'isAdmin') con los claims que nosotros definimos ('email' y 'lo que yo quiera').
        claims.AddRange(claimsDB);

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
