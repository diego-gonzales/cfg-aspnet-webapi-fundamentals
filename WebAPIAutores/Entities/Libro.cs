namespace WebAPIAutores;

public class Libro
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int AutorId { get; set; }

    // propiedad de navegación con que voy a poder cargar desde un libro, la data de autor que escribió el libro
    public Autor Autor { get; set; }
}
