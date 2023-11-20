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
        var authorDto = result.Value as AuthorDTO;

        if (authorDto == null)
        {
            var authorDtos =
                result.Value as List<AuthorDTO>
                ?? throw new ArgumentException(
                    "Se esperaba una instancia de 'AuthorDTO' o 'List<AuthorDTO>'"
                );

            authorDtos.ForEach(async authorDto => await linksGenerator.GenerateLinks(authorDto));

            result.Value = authorDtos;
        }
        else
        {
            await linksGenerator.GenerateLinks(authorDto);
        }

        await next();
    }
}
