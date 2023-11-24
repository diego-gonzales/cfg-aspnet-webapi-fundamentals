using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
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

    [TestMethod]
    public async Task IfUserIsNotAdmin_Get2Links_UsingMoq()
    {
        // con la ayuda de la librería 'MOQ' creamos el mock para 'IAuthorizationService'
        var authorizationServiceMock = new Mock<IAuthorizationService>();
        authorizationServiceMock
            .Setup(
                x =>
                    x.AuthorizeAsync(
                        It.IsAny<ClaimsPrincipal>(),
                        It.IsAny<object>(),
                        It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                    )
            )
            .Returns(Task.FromResult(AuthorizationResult.Failed()));
        authorizationServiceMock
            .Setup(
                x =>
                    x.AuthorizeAsync(
                        It.IsAny<ClaimsPrincipal>(),
                        It.IsAny<object>(),
                        It.IsAny<string>()
                    )
            )
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        // con la ayuda de la librería 'MOQ' creamos el mock para 'IUrlHelper'
        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock
            .Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(string.Empty);

        var rootController = new RootController(authorizationServiceMock.Object)
        {
            Url = urlHelperMock.Object
        };

        var result = await rootController.Get();

        Assert.AreEqual(2, result.Value.Count());
    }
}
