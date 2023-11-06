using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresResourceManipulation;

[ApiController]
[Route("api/autores")]
public class AutoresController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public AutoresController(ApplicationDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<Autor>>> Get()
    {
        return await dbContext.Autores.ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Autor>> GetOne(int id)
    {
        var autor = await dbContext.Autores.FirstOrDefaultAsync(x => x.Id == id);

        if (autor == null)
        {
            return NotFound();
        }

        return autor;
    }

    [HttpPost]
    public async Task<ActionResult> Post(CreateAuthorDTO createAuthorDTO)
    {
        var autorAlreadyExists = await dbContext.Autores.AnyAsync(
            x => x.Name == createAuthorDTO.Name
        );

        if (autorAlreadyExists)
        {
            return BadRequest($"Ya existe un autor con el nombre {createAuthorDTO.Name}");
        }

        var author = mapper.Map<Autor>(createAuthorDTO);

        dbContext.Add(author);
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, Autor autor)
    {
        if (autor.Id != id)
        {
            return BadRequest("El id del autor no coincide con el id de la URL");
        }

        var autorExists = await dbContext.Autores.AnyAsync(x => x.Id == id);

        if (!autorExists)
        {
            return NotFound();
        }

        dbContext.Update(autor);
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var autorExists = await dbContext.Autores.AnyAsync(x => x.Id == id);

        if (!autorExists)
        {
            return NotFound();
        }

        dbContext.Remove(new Autor { Id = id });
        await dbContext.SaveChangesAsync();
        return Ok();
    }
}
