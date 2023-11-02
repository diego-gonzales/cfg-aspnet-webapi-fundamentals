using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutores;

[ApiController]
// [Authorize]
[Route("api/autores")]
// [Route("api/[controller]")] --> toma dinámicamente el nombre de la clase sin el sufijo 'Controller', es decir 'Autores'
public class AutoresController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly ServicioTransient servicioTransient;
    private readonly ServicioScoped servicioScoped;
    private readonly ServicioSingleton servicioSingleton;
    private readonly IServicio servicio;
    private readonly ILogger<AutoresController> logger;

    public AutoresController(
        ApplicationDbContext dbContext,
        ServicioTransient servicioTransient,
        ServicioScoped servicioScoped,
        ServicioSingleton servicioSingleton,
        IServicio servicio,
        ILogger<AutoresController> logger // Este es uno de los servicios básicos que ASP.NET Core prepara por defecto, por lo tanto no necesitamos añadirlo en el 'Startup.cs' como servicio.
    )
    {
        this.dbContext = dbContext;
        this.servicioTransient = servicioTransient;
        this.servicioScoped = servicioScoped;
        this.servicioSingleton = servicioSingleton;
        this.servicio = servicio;
        this.logger = logger;
    }

    [HttpGet("guid")]
    [ResponseCache(Duration = 10)] // Filtro que sirve para que las request que lleguen 10s despues sean servidas de cache.
    // [Authorize] // Filtro que sirve para proteger mi endpoint
    [ServiceFilter(typeof(MyActionFilter))] // Way to use a custom filter
    public ActionResult GetGuid()
    {
        return Ok(
            new
            {
                AutoresController_Transient = servicioTransient.Guid,
                ServicioA_Transient = servicio.GetTransient(),
                AutoresController_Scoped = servicioScoped.Guid,
                ServicioA_Scoped = servicio.GetScoped(),
                AutoresController_Singleton = servicioSingleton.Guid,
                ServicioA_Singleton = servicio.GetSingleton(),
            }
        );
    }

    [HttpGet] // --> api/autores
    // [HttpGet("listado")] // --> api/autores/listado
    // [HttpGet("/listado-autores")] // --> /listado-autores
    public async Task<ActionResult<List<Autor>>> Get()
    {
        // este log NO se mostrará debido a que en 'appsettings.development.json' configuramos que solo se procecen LOGS > 'WARNING' para el namespace de 'WebAPIAutores' (WARNING, ERROR, CRITICAL). Por defecto se muestran LOGS > 'INFORMATION' (INFORMATION, WARNING, ERROR, CRITICAL), pero como le indicamos otra cosa para el namespace 'WebAPIAutores' entonces tiene precedencia sobre este.
        logger.LogInformation("Information Log!!!");
        // entonces este log SÍ se mostrará
        logger.LogWarning("Warning Log!!!");
        return await dbContext.Autores.Include(x => x.Libros).ToListAsync();
    }

    [HttpGet("{id:int}")]
    // [HttpGet("{id:int}/{param2?}")] // --> parámetro opcional
    // [HttpGet("{id:int}/{param2=valordeprueba}")] // --> parámetro por defecto
    public async Task<ActionResult<Autor>> GetOne(int id, string param2)
    {
        var autor = await dbContext.Autores
            .Include(x => x.Libros)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (autor == null)
        {
            return NotFound();
        }

        return autor;
    }

    [HttpGet("{nombre}")]
    public async Task<ActionResult<Autor>> GetByName([FromRoute] string nombre) // no es necesario especificar el [FromRoute]
    {
        // throw new NotImplementedException(); // 👀👀👀 this is only to try the 'MyExceptionFilter' global filter

        var autor = await dbContext.Autores
            .Include(x => x.Libros)
            .FirstOrDefaultAsync(x => x.Name.Contains(nombre));

        if (autor == null)
        {
            return NotFound();
        }

        return autor;
    }

    [HttpGet("first")] // api/autores/first?nombre=diego
    public async Task<ActionResult<Autor>> GetFirst(
        [FromHeader] int miValor,
        [FromQuery] string nombre
    )
    {
        return await dbContext.Autores.Include(x => x.Libros).FirstOrDefaultAsync();
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Autor autor) // no es necesario especificar el [FromBody]
    {
        var existeAutor = await dbContext.Autores.AnyAsync(x => x.Name == autor.Name);

        if (existeAutor)
        {
            return BadRequest($"Ya existe un autor con el nombre {autor.Name}");
        }

        dbContext.Add(autor);
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

        var authorExists = await dbContext.Autores.AnyAsync(x => x.Id == id);

        if (!authorExists)
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
        var authorExists = await dbContext.Autores.AnyAsync(x => x.Id == id);

        if (!authorExists)
        {
            return NotFound();
        }

        dbContext.Remove(new Autor { Id = id });
        await dbContext.SaveChangesAsync();
        return Ok();
    }
}
