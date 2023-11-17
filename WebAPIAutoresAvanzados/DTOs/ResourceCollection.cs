namespace WebAPIAutoresAvanzados;

// 'T' es un tipo genérico, y aquí le digo que 'T' debe heredar de Resource (en este caso por ejemplo le pasaríamos 'AuthorDTO' como genérico, el cual sí hereda de 'Resource')
public class ResourceCollection<T> : Resource
    where T : Resource
{
    public List<T> Data { get; set; }
}
