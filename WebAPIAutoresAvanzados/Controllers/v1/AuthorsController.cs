using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresAvanzados.Controllers.v1;

[ApiController]
[Route("api/v1/authors")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiConventionType(typeof(DefaultApiConventions))] // para que swagger sepa que convenciones debo usar. Es una alternativa a colocar [ProducesResponseType(200)] en cada endpoint. La podemos colocar de manera global (ver Startup.cs)
public class AuthorsController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IAuthorizationService authorizationService;

    public AuthorsController(
        ApplicationDbContext dbContext,
        IMapper mapper,
        IAuthorizationService authorizationService
    )
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.authorizationService = authorizationService;
    }

    [HttpGet(Name = "getAuthors")]
    [AllowAnonymous] // me permite consultar el endpoint sin necesidad de authorization (JWT)
    [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
    public async Task<ActionResult<List<AuthorDTO>>> Get()
    {
        var authors = await dbContext.Autores.ToListAsync();
        return mapper.Map<List<AuthorDTO>>(authors);
    }

    [HttpGet("{id:int}", Name = "getAuthor")]
    [AllowAnonymous]
    [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
    [ProducesResponseType(200)] // indica que el endpoint devuelve un 200. Es opcional, sirve para documentar (Swagger)
    [ProducesResponseType(404)] // indica que el endpoint devuelve un 404. Es opcional, sirve para documentar (Swagger)
    public async Task<ActionResult<AuthorWithBooksDTO>> GetOne(int id)
    {
        var author = await dbContext.Autores
            .Include(author => author.AuthorsBooks)
            .ThenInclude(authorBook => authorBook.Book)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (author == null)
        {
            return NotFound();
        }

        return mapper.Map<AuthorWithBooksDTO>(author);
    }

    [HttpPost(Name = "createAuthor")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult> Post(CreateAuthorDTO createAuthorDTO)
    {
        var autorAlreadyExists = await dbContext.Autores.AnyAsync(
            x => x.Name == createAuthorDTO.Name
        );

        if (autorAlreadyExists)
        {
            return BadRequest($"Ya existe un autor con el nombre {createAuthorDTO.Name}");
        }

        var author = mapper.Map<Author>(createAuthorDTO);

        dbContext.Add(author);
        await dbContext.SaveChangesAsync();

        var authorDto = mapper.Map<AuthorDTO>(author);

        return CreatedAtRoute("getAuthor", new { id = author.Id }, authorDto);
    }

    [HttpPut("{id:int}", Name = "updateAuthor")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult> Put(int id, UpdateAuthorDTO updateAuthorDTO)
    {
        var autorExists = await dbContext.Autores.AnyAsync(x => x.Id == id);

        if (!autorExists)
        {
            return NotFound();
        }

        var author = mapper.Map<Author>(updateAuthorDTO);
        author.Id = id;

        dbContext.Update(author);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Remove an author
    /// </summary>
    /// <param name="id">Author ID to remove</param>
    /// <returns></returns>
    [HttpDelete("{id:int}", Name = "deleteAuthor")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult> Delete(int id)
    {
        var autorExists = await dbContext.Autores.AnyAsync(x => x.Id == id);

        if (!autorExists)
        {
            return NotFound();
        }

        dbContext.Remove(new Author { Id = id });
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{name}", Name = "getAuthorByName")]
    public async Task<ActionResult<List<AuthorDTO>>> GetByName(string name)
    {
        var authors = await dbContext.Autores.Where(x => x.Name.Contains(name)).ToListAsync();
        return mapper.Map<List<AuthorDTO>>(authors);
    }
}
