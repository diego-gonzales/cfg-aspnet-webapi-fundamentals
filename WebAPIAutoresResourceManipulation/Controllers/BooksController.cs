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
    public async Task<ActionResult<BookWithCommentsAndAuthorsDTO>> GetOne(int id)
    {
        var book = await dbContext.Libros
            .Include(book => book.Comments)
            .Include(book => book.AuthorsBooks)
            .ThenInclude(authorBook => authorBook.Author)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        book.AuthorsBooks = book.AuthorsBooks.OrderBy(x => x.Order).ToList();

        return mapper.Map<BookWithCommentsAndAuthorsDTO>(book);
    }

    [HttpPost]
    public async Task<ActionResult> Post(CreateBookDTO createBookDTO)
    {
        if (createBookDTO.AuthorIds == null)
        {
            return BadRequest("No se puede crear un libro sin autores");
        }

        var authorIds = await dbContext.Autores
            .Where(a => createBookDTO.AuthorIds.Contains(a.Id))
            .Select(x => x.Id)
            .ToListAsync();

        if (createBookDTO.AuthorIds.Count != authorIds.Count)
        {
            return BadRequest("Uno de los autores enviados no existe");
        }

        var book = mapper.Map<Book>(createBookDTO);

        if (book.AuthorsBooks != null)
        {
            for (int i = 0; i < book.AuthorsBooks.Count; i++)
            {
                book.AuthorsBooks[i].Order = i;
            }
        }

        dbContext.Add(book);
        await dbContext.SaveChangesAsync();
        return Ok();
    }
}
