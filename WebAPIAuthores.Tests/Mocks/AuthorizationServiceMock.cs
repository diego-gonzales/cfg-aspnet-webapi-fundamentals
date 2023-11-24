using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WebAPIAuthores.Tests;

public class AuthorizationServiceMock : IAuthorizationService
{
    public AuthorizationResult Resultado { get; set; }

    public Task<AuthorizationResult> AuthorizeAsync(
        ClaimsPrincipal user,
        object resource,
        IEnumerable<IAuthorizationRequirement> requirements
    )
    {
        // return Task.FromResult(AuthorizationResult.Success());
        return Task.FromResult(Resultado);
    }

    public Task<AuthorizationResult> AuthorizeAsync(
        ClaimsPrincipal user,
        object resource,
        string policyName
    )
    {
        // return Task.FromResult(AuthorizationResult.Success());
        return Task.FromResult(Resultado);
    }
}
