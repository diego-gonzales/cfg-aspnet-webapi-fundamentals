using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace WebAPIAuthores.Tests;

public class UrlHelperMock : IUrlHelper
{
    public ActionContext ActionContext => throw new NotImplementedException();

    public string Action(UrlActionContext actionContext)
    {
        throw new NotImplementedException();
    }

    [return: NotNullIfNotNull("contentPath")]
    public string Content(string contentPath)
    {
        throw new NotImplementedException();
    }

    public bool IsLocalUrl([NotNullWhen(true)] string url)
    {
        throw new NotImplementedException();
    }

    // Es el único método que me interesa para mi mock, ya que los usamos aquí: new HATEOASData(link: Url.Link("GetRoot", new { }), description: "self", method: "GET"), es por eso que es el único que modificamos, en este caso solo enviamos un string vacío ya que solo necesitamos que no devuelva un error, cuando llamamos el 'Url.Link'
    public string Link(string routeName, object values)
    {
        return "";
    }

    public string RouteUrl(UrlRouteContext routeContext)
    {
        throw new NotImplementedException();
    }
}
