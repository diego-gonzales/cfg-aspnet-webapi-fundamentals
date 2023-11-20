using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIAutoresAvanzados;

[ApiController]
[Route("api/v1")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RootController : ControllerBase
{
    private readonly IAuthorizationService authorizationService;

    public RootController(IAuthorizationService authorizationService)
    {
        this.authorizationService = authorizationService;
    }

    [HttpGet(Name = "GetRoot")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<HATEOASData>>> Get()
    {
        var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");

        var hateoasData = new List<HATEOASData>();

        hateoasData.Add(
            new HATEOASData(link: Url.Link("GetRoot", new { }), description: "self", method: "GET")
        );
        hateoasData.Add(
            new HATEOASData(
                link: Url.Link("getAuthors", new { }),
                description: "Get author list",
                method: "GET"
            )
        );

        if (isAdmin.Succeeded)
        {
            hateoasData.Add(
                new HATEOASData(
                    link: Url.Link("createAuthor", new { }),
                    description: "Create an author",
                    method: "POST"
                )
            );
            hateoasData.Add(
                new HATEOASData(
                    link: Url.Link("createBook", new { }),
                    description: "Create a book",
                    method: "POST"
                )
            );
        }

        return hateoasData;
    }
}
