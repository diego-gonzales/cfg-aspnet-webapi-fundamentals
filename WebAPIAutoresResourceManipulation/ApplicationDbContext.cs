using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresResourceManipulation;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Author> Autores { get; set; }
    public DbSet<Book> Libros { get; set; }
    public DbSet<Comment> Comments { get; set; }
}
