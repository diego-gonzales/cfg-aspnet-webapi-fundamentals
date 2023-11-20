using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace WebAPIAutoresAvanzados;

public class LinksGenerator
{
    private readonly IAuthorizationService authorizationService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IActionContextAccessor actionContextAccessor;

    public LinksGenerator(
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor,
        IActionContextAccessor actionContextAccessor
    )
    {
        this.authorizationService = authorizationService;
        this.httpContextAccessor = httpContextAccessor;
        this.actionContextAccessor = actionContextAccessor;
    }

    public async Task GenerateLinks(AuthorDTO authorDTO)
    {
        var isAdmin = await IsAdmin();
        var url = BuildURLHelper();

        authorDTO.Links.Add(
            new HATEOASData(
                link: url.Link("getAuthor", new { id = authorDTO.Id }),
                description: "Get author detail",
                method: "GET"
            )
        );

        if (isAdmin)
        {
            authorDTO.Links.Add(
                new HATEOASData(
                    link: url.Link("updateAuthor", new { id = authorDTO.Id }),
                    description: "Update an author",
                    method: "PUT"
                )
            );

            authorDTO.Links.Add(
                new HATEOASData(
                    link: url.Link("deleteAuthor", new { id = authorDTO.Id }),
                    description: "Delete an author",
                    method: "DELETE"
                )
            );
        }
    }

    private IUrlHelper BuildURLHelper()
    {
        var factory =
            httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
        return factory.GetUrlHelper(actionContextAccessor.ActionContext);
    }

    private async Task<bool> IsAdmin()
    {
        var httpContext = httpContextAccessor.HttpContext;

        var result = await authorizationService.AuthorizeAsync(
            user: httpContext.User,
            policyName: "isAdmin"
        );
        return result.Succeeded;
    }
}
