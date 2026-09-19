using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TechLogistics.Shared.Auth;

namespace TechLogistics.Client.Auth;

/// <summary>
/// AuthenticationStateProvider del cliente InteractiveWebAssembly. Recupera el
/// <see cref="UserInfo"/> que el servidor persistió al final del renderizado estático.
/// </summary>
public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> DefaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> _authenticationStateTask = DefaultUnauthenticatedTask;

    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<UserInfo>("userInfo", out var userInfo) || userInfo is null)
        {
            return;
        }

        var claims = new List<Claim> { new(ClaimTypes.Name, userInfo.UserName) };
        claims.AddRange(userInfo.Roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

        var identity = new ClaimsIdentity(claims, authenticationType: "TechLogisticsPersisted");
        _authenticationStateTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authenticationStateTask;
}
