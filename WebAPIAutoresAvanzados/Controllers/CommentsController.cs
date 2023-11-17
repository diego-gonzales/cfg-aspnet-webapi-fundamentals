using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresAvanzados;

[ApiController]
[Route("api/books/{bookId}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly UserManager<IdentityUser> userManager;

    public CommentsController(
        ApplicationDbContext dbContext,
        IMapper mapper,
        UserManager<IdentityUser> userManager
    )
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.userManager = userManager;
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

    [HttpGet("/api/comments/{id:int}", Name = "getComment")]
    public async Task<ActionResult<CommentWithBookDTO>> GetOne(int id)
    {
        var comment = await dbContext.Comments
            .Include(x => x.Book)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (comment == null)
        {
            return NotFound();
        }

        return mapper.Map<CommentWithBookDTO>(comment);
    }

    // NOTA: Si por ejemplo queremos permitir que usuarios anónimos escriban comentarios, podríamos colocar el [Authorize] a nivel de controlador, y aquí en el método 'Post' podríamos colocar el [AllowAnonymous] (es decir que no va a requerir autenticación: JWT), y hacer una validación, ya que el 'emailClaim' sería null si es que escribe usuario anónimo, de tal manera que solo agregaríamos el 'userId' si es diferente de null.
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Post(int bookId, CreateCommentDTO createCommentDTO)
    {
        // Este "email" es el claim que se agrega en el método GenerateToken(), lo obtengo de esta manera porque en el Startup.cs hice la siguiente configuración: JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(), de lo contrario tendría que hacerlo de esta manera: var email = HttpContext.User.Claims.Where(claim => claim.Type == ClaimTypes.Email).FirstOrDefault();. Ambas son válidas.
        var emailClaim = HttpContext.User.Claims
            .Where(claim => claim.Type == "email")
            .FirstOrDefault();
        var email = emailClaim.Value;
        var user = await userManager.FindByEmailAsync(email);
        var userId = user.Id;

        var bookExists = await dbContext.Libros.AnyAsync(x => x.Id == bookId);

        if (!bookExists)
        {
            return NotFound();
        }

        var comment = mapper.Map<Comment>(createCommentDTO);
        comment.BookId = bookId;
        comment.UserId = userId;

        dbContext.Add(comment);
        await dbContext.SaveChangesAsync();

        var commentDto = mapper.Map<CommentDTO>(comment);

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

    [HttpDelete("/api/comments/{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var comment = await dbContext.Comments.FirstOrDefaultAsync(x => x.Id == id);

        if (comment == null)
        {
            return NotFound();
        }

        dbContext.Remove(comment);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
