using Microsoft.AspNetCore.Authorization;
using WebAPIAutoresAvanzados.Controllers.v1;

namespace WebAPIAuthores.Tests;

[TestClass]
public class RootControllerTests
{
    [TestMethod]
    public async Task IfUserIsAdmin_Get4Links()
    {
        // PREPARAR
        var authorizationServiceMock = new AuthorizationServiceMock
        {
            Resultado = AuthorizationResult.Success()
        };
        // authorizationServiceMock.Resultado = AuthorizationResult.Success();

        var rootController = new RootController(authorizationServiceMock)
        {
            Url = new UrlHelperMock()
        };
        // rootController.Url = new UrlHelperMock(); // también es válido.

        // PROBAR
        var result = await rootController.Get();

        // VERIFICAR
        Assert.AreEqual(4, result.Value.Count());
    }

    [TestMethod]
    public async Task IfUserIsNotAdmin_Get2Links()
    {
        var authorizationServiceMock = new AuthorizationServiceMock
        {
            Resultado = AuthorizationResult.Failed()
        };

        var rootController = new RootController(authorizationServiceMock)
        {
            Url = new UrlHelperMock()
        };

        var result = await rootController.Get();

        Assert.AreEqual(2, result.Value.Count());
    }
}
