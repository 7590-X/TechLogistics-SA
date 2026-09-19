using System.Security.Claims;

namespace TechLogistics.Web.Auth;

public interface IJwtTokenService
{
    string GenerarToken(string usuario, string rol);
    ClaimsPrincipal? ValidarToken(string token);
}
