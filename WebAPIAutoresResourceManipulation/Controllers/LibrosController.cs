using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresResourceManipulation;

[ApiController]
[Route("api/libros")]
public class LibrosController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;

    public LibrosController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<Libro>>> Get()
    {
        return await dbContext.Libros.ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Libro>> GetOne(int id)
    {
        var libro = await dbContext.Libros.FirstOrDefaultAsync(x => x.Id == id);

        if (libro == null)
        {
            return NotFound();
        }

        return libro;
    }

    [HttpPost]
    public async Task<ActionResult> Post(Libro libro)
    {
        bool authorExists = await dbContext.Autores.AnyAsync(x => x.Id == libro.AutorId);

        if (!authorExists)
        {
            return BadRequest($"No existe el autor con el id {libro.AutorId}");
        }

        dbContext.Add(libro);
        await dbContext.SaveChangesAsync();
        return Ok();
    }
}
