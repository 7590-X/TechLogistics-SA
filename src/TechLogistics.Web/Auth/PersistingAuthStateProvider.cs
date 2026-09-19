using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TechLogistics.Shared.Auth;
using Microsoft.AspNetCore.Components.Web;

namespace TechLogistics.Web.Auth;

public class PersistingAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly PersistentComponentState _persistentState;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PersistingComponentStateSubscription _subscription;

    private Task<AuthenticationState>? _authenticationStateTask;

    public PersistingAuthStateProvider(PersistentComponentState persistentState, IHttpContextAccessor httpContextAccessor)
    {
        _persistentState = persistentState;
        _httpContextAccessor = httpContextAccessor;
        _subscription = _persistentState.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var usuario = _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(usuario));
        _authenticationStateTask = authState;
        return authState;
    }

    private async Task OnPersistingAsync()
    {
        if (_authenticationStateTask is null)
        {
            throw new InvalidOperationException($"{nameof(OnPersistingAsync)} fue invocado antes de {nameof(GetAuthenticationStateAsync)}.");
        }

        var authState = await _authenticationStateTask;
        var principal = authState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
            _persistentState.PersistAsJson("userInfo", new UserInfo(principal.Identity.Name ?? string.Empty, roles));
        }
    }

    public void Dispose() => _subscription.Dispose();
}
