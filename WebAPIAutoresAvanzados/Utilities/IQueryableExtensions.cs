namespace WebAPIAutoresAvanzados;

public static class IQueryableExtensions
{
    public static IQueryable<T> Paginate<T>(
        this IQueryable<T> queryable, // IQueryable<T> es una interfaz que permite hacer consultas a la base de datos, 'this' es para extender el IQueryable<T>
        PaginationDTO paginationDTO
    )
    {
        return queryable
            .Skip((paginationDTO.Page - 1) * paginationDTO.PerPage)
            .Take(paginationDTO.PerPage);
    }
}
