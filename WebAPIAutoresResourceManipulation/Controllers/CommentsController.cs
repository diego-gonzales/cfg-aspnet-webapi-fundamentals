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

    [HttpGet("{id:int}", Name = "getComment")]
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
        return CreatedAtRoute("getComment", new { bookId = bookId, id = comment.Id }, commentDto);
    }
}
