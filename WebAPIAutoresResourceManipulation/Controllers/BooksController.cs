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

    [HttpGet("{id:int}", Name = "getBook")]
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

        AssignOrderToAuthors(book);

        dbContext.Add(book);
        await dbContext.SaveChangesAsync();

        var bookDto = mapper.Map<BookDTO>(book);

        // params: routeName, routeValue (anonymous object), value
        return CreatedAtRoute("getBook", new { id = book.Id }, bookDto);
    }

    /* Esta es otra forma de hacer una actualización. Se hizo de esta manera porque así se mantiene la misma instancia de bookFromDB, lo que permite lograr que EF Core haga la actualización de los campos cuando hacemos SaveChangesAsync. Es básicamente un truco que muestra Felipe Gávilan para actualizar una entidad y sus entidades relacionadas de una manera rápida. */
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, CreateBookDTO createBookDTO)
    {
        var bookFromDB = await dbContext.Libros
            .Include(x => x.AuthorsBooks)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (bookFromDB == null)
        {
            return NotFound();
        }

        // está parte se explica aquí: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/26946934#questions/19495216, en el minuto 3:00, donde básicamente comenta que usamos la misma instancia de bookFromDB que se encuentra en memoria para actualizar la data.
        bookFromDB = mapper.Map(createBookDTO, bookFromDB);

        AssignOrderToAuthors(bookFromDB);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private void AssignOrderToAuthors(Book book)
    {
        if (book.AuthorsBooks != null)
        {
            for (int i = 0; i < book.AuthorsBooks.Count; i++)
            {
                book.AuthorsBooks[i].Order = i;
            }
        }
    }
}
