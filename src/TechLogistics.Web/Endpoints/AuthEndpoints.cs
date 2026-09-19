using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TechLogistics.Web.Auth;

namespace TechLogistics.Web.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (HttpContext http, IJwtTokenService jwt, DemoUserStore users) =>
        {
            var form = await http.Request.ReadFormAsync();
            var usuario = form["usuario"].ToString();
            var password = form["password"].ToString();
            var returnUrl = form["returnUrl"].ToString();

            var rol = users.ValidarCredenciales(usuario, password);
            if (rol is null)
            {
                return Results.Redirect($"/login?error=1&returnUrl={Uri.EscapeDataString(returnUrl)}");
            }

            var token = jwt.GenerarToken(usuario, rol);
            var principal = jwt.ValidarToken(token)
                ?? throw new InvalidOperationException("El token recién emitido no pudo validarse.");

            await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

            var destino = string.IsNullOrWhiteSpace(returnUrl)
                ? (rol == "GerenteBodega" ? "/bodega/monitoreo" : "/despacho/escaneo")
                : returnUrl;

            return Results.Redirect(destino);
        }).DisableAntiforgery();

        app.MapPost("/api/auth/logout", async (HttpContext http) =>
        {
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/");
        }).DisableAntiforgery();
    }
}
