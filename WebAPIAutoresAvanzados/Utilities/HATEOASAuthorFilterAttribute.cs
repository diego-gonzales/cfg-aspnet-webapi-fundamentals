using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPIAutoresAvanzados;

public class HATEOASAuthorFilterAttribute : HATEOASFilterAttribute
{
    private readonly LinksGenerator linksGenerator;

    public HATEOASAuthorFilterAttribute(LinksGenerator linksGenerator)
    {
        this.linksGenerator = linksGenerator;
    }

    public override async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next
    )
    {
        var shouldInclude = ShouldIncludeHATEOAS(context);

        if (!shouldInclude)
        {
            await next();
            return;
        }

        var result = context.Result as ObjectResult;
        var model =
            result.Value as AuthorDTO
            ?? throw new ArgumentNullException("Se esperaba una instancia de AuthorDTO");

        await linksGenerator.GenerateLinks(model);
        await next();
    }
}
