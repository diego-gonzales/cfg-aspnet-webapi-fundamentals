using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresAvanzados;

[ApiController]
[Route("api/authors")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
    public async Task<ActionResult<ResourceCollection<AuthorDTO>>> Get()
    {
        var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");

        var authors = await dbContext.Autores.ToListAsync();
        var authorDtos = mapper.Map<List<AuthorDTO>>(authors);

        authorDtos.ForEach(authorDto => GenerateLinks(authorDto, isAdmin.Succeeded));

        var resourceCollection = new ResourceCollection<AuthorDTO> { Data = authorDtos };

        resourceCollection.Links.Add(
            new HATEOASData(
                link: Url.Link("getAuthors", new { }),
                description: "Get author list",
                method: "GET"
            )
        );

        if (isAdmin.Succeeded)
        {
            resourceCollection.Links.Add(
                new HATEOASData(
                    link: Url.Link("createAuthor", new { }),
                    description: "Create an author",
                    method: "POST"
                )
            );
        }

        return resourceCollection;
    }

    [HttpGet("{id:int}", Name = "getAuthor")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthorWithBooksDTO>> GetOne(int id)
    {
        var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");

        var author = await dbContext.Autores
            .Include(author => author.AuthorsBooks)
            .ThenInclude(authorBook => authorBook.Book)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (author == null)
        {
            return NotFound();
        }

        var authorWithBooksDTO = mapper.Map<AuthorWithBooksDTO>(author);

        GenerateLinks(authorWithBooksDTO, isAdmin.Succeeded);

        return authorWithBooksDTO;
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

    private void GenerateLinks(AuthorDTO authorDTO, bool isAdmin)
    {
        authorDTO.Links.Add(
            new HATEOASData(
                link: Url.Link("getAuthor", new { id = authorDTO.Id }),
                description: "Get author detail",
                method: "GET"
            )
        );

        if (isAdmin)
        {
            authorDTO.Links.Add(
                new HATEOASData(
                    link: Url.Link("updateAuthor", new { id = authorDTO.Id }),
                    description: "Update an author",
                    method: "PUT"
                )
            );

            authorDTO.Links.Add(
                new HATEOASData(
                    link: Url.Link("deleteAuthor", new { id = authorDTO.Id }),
                    description: "Delete an author",
                    method: "DELETE"
                )
            );
        }
    }
}
