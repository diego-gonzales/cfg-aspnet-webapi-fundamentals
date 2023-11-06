using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresResourceManipulation;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public BooksController(ApplicationDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookDTO>>> Get()
    {
        var books = await dbContext.Libros.ToListAsync();
        return mapper.Map<List<BookDTO>>(books);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDTO>> GetOne(int id)
    {
        var book = await dbContext.Libros.FirstOrDefaultAsync(x => x.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return mapper.Map<BookDTO>(book);
    }

    [HttpPost]
    public async Task<ActionResult> Post(CreateBookDTO createBookDTO)
    {
        // bool authorExists = await dbContext.Autores.AnyAsync(x => x.Id == libro.AutorId);

        // if (!authorExists)
        // {
        //     return BadRequest($"No existe el autor con el id {libro.AutorId}");
        // }

        var book = mapper.Map<Book>(createBookDTO);

        dbContext.Add(book);
        await dbContext.SaveChangesAsync();
        return Ok();
    }
}
