using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresAvanzados;

public static class HttpContextExtensions
{
    public static async Task InsertPaginationParameterInHeader<T>(
        this HttpContext httpContext, // this is the extension method for HttpContext. 'this' is the first parameter
        IQueryable<T> queryable
    )
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        double total = await queryable.CountAsync();
        httpContext.Response.Headers.Add("totalItems", total.ToString());
    }
}
