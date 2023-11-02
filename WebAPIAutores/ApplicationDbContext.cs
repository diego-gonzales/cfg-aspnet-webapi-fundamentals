using Microsoft.EntityFrameworkCore;

namespace WebAPIAutores;
public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions options) : base(options)
  {
  }

  public DbSet<Autor> Autores { get; set; }
  // El DbSet de libros es opcional ya que Autores tiene una propiedad que se relaciona con Libros y por lo tanto EF creará la tabla de Libros automáticamente, pero si queremos hacer querys a la tabla de Libros si debemos colocarlo, de lo contrario tendríamos que trabajar a través de la tabla de libros dichas querys.
  public DbSet<Libro> Libros { get; set; }
}
