using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresAvanzados;

[ApiController]
[Route("api/v1/books")]
public class BooksController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public BooksController(ApplicationDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    [HttpGet(Name = "getBooks")]
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

    [HttpPost(Name = "createBook")]
    public async Task<ActionResult> Post(CreateBookDTO createBookDTO)
    {
        var validationResult = await ValidateAuthors(createBookDTO);

        if (validationResult != null)
        {
            return validationResult;
        }

        var book = mapper.Map<Book>(createBookDTO);

        AssignOrderToAuthors(book);

        dbContext.Add(book);
        await dbContext.SaveChangesAsync();

        var bookDto = mapper.Map<BookDTO>(book);

        return CreatedAtRoute("getBook", new { id = book.Id }, bookDto);
    }

    [HttpPut("{id:int}", Name = "updateBook")]
    public async Task<ActionResult> Put(int id, CreateBookDTO createBookDTO)
    {
        var validationResult = await ValidateAuthors(createBookDTO);

        if (validationResult != null)
        {
            return validationResult;
        }

        var bookFromDB = await dbContext.Libros
            .Include(x => x.AuthorsBooks)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (bookFromDB == null)
        {
            return NotFound();
        }

        bookFromDB = mapper.Map(createBookDTO, bookFromDB);

        AssignOrderToAuthors(bookFromDB);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "patchBook")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<PatchBookDTO> jsonPatchDocument)
    {
        if (jsonPatchDocument == null)
        {
            return BadRequest();
        }

        var bookFromDB = await dbContext.Libros.FirstOrDefaultAsync(x => x.Id == id);

        if (bookFromDB == null)
        {
            return NotFound();
        }

        var patchBookDto = mapper.Map<PatchBookDTO>(bookFromDB);

        jsonPatchDocument.ApplyTo(patchBookDto, ModelState);

        var isValid = TryValidateModel(patchBookDto);

        if (!isValid)
        {
            return BadRequest(ModelState);
        }

        mapper.Map(patchBookDto, bookFromDB);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "deleteBook")]
    public async Task<ActionResult> Delete(int id)
    {
        var book = await dbContext.Libros.FirstOrDefaultAsync(x => x.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        dbContext.Remove(book);
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

    private async Task<ActionResult> ValidateAuthors(CreateBookDTO createBookDTO)
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

        return null;
    }
}
