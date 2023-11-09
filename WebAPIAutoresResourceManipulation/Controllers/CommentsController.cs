using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresResourceManipulation;

[ApiController]
[Route("api/books/{bookId}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public CommentsController(ApplicationDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<CommentDTO>>> Get(int bookId)
    {
        var bookExists = await dbContext.Libros.AnyAsync(x => x.Id == bookId);

        if (!bookExists)
        {
            return NotFound();
        }

        var comments = await dbContext.Comments.Where(x => x.BookId == bookId).ToListAsync();
        return mapper.Map<List<CommentDTO>>(comments);
    }

    // // ruta ---> api/books/{bookId}/comments/{id}, I change this route because the {bookId} paramenter is not used.
    // [HttpGet("{id:int}", Name = "getComment")]
    [HttpGet("/api/comments/{id:int}", Name = "getComment")]
    public async Task<ActionResult<CommentWithBookDTO>> GetOne(int id)
    {
        var comment = await dbContext.Comments
            .Include(x => x.Book)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (comment == null)
        {
            return NotFound();
        }

        return mapper.Map<CommentWithBookDTO>(comment);
    }

    [HttpPost]
    public async Task<ActionResult> Post(int bookId, CreateCommentDTO createCommentDTO)
    {
        var bookExists = await dbContext.Libros.AnyAsync(x => x.Id == bookId);

        if (!bookExists)
        {
            return NotFound();
        }

        var comment = mapper.Map<Comment>(createCommentDTO);
        comment.BookId = bookId;

        dbContext.Add(comment);
        await dbContext.SaveChangesAsync();

        var commentDto = mapper.Map<CommentDTO>(comment);

        // en este caso el routeValues tiene dos valores: el bookId (del atributo Route) y el id (del atributo HttpGet)
        // return CreatedAtRoute("getComment", new { bookId = bookId, id = comment.Id }, commentDto);
        return CreatedAtRoute("getComment", new { id = comment.Id }, commentDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int bookId, int id, UpdateCommentDTO updateCommentDTO)
    {
        var bookExists = await dbContext.Libros.AnyAsync(x => x.Id == bookId);

        if (!bookExists)
        {
            return NotFound($"Libro con el id {bookId} no existe");
        }

        var commentExists = await dbContext.Comments.AnyAsync(x => x.Id == id);

        if (!commentExists)
        {
            return NotFound($"Comentario con el id {id} no existe");
        }

        // extra validation to validate that a comment belongs to a book, otherwise this comment would be moved to another book and removed from the current book.
        var commentBelongsToBook = await dbContext.Comments.AnyAsync(
            comment => comment.Id == id && comment.BookId == bookId
        );

        if (!commentBelongsToBook)
        {
            return NotFound($"Comentario {id} no pertenece al libro {bookId}");
        }

        var comment = mapper.Map<Comment>(updateCommentDTO);
        comment.BookId = bookId;
        comment.Id = id;

        dbContext.Update(comment);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
