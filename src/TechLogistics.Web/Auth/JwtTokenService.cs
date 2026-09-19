using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TechLogistics.Web.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerarToken(string usuario, string rol)
    {
        var claves = ObtenerParametrosJwt();
        var credenciales = new SigningCredentials(claves.ClaveFirma, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, usuario),
            new(ClaimTypes.Role, rol),
            new(JwtRegisteredClaimNames.Sub, usuario),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: claves.Issuer,
            audience: claves.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(claves.ExpiryMinutes),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidarToken(string token)
    {
        var claves = ObtenerParametrosJwt();
        var handler = new JwtSecurityTokenHandler();

        try
        {
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = claves.Issuer,
                ValidateAudience = true,
                ValidAudience = claves.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = claves.ClaveFirma,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            }, out _);

            return principal;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
    }

    private (SymmetricSecurityKey ClaveFirma, string Issuer, string Audience, int ExpiryMinutes) ObtenerParametrosJwt()
    {
        var seccion = _configuration.GetSection("Jwt");
        var key = seccion["Key"] ?? throw new InvalidOperationException("Falta configurar Jwt:Key en appsettings.json.");
        var issuer = seccion["Issuer"] ?? "TechLogistics";
        var audience = seccion["Audience"] ?? "TechLogisticsPortal";
        var expiry = int.TryParse(seccion["ExpiryMinutes"], out var m) ? m : 60;

        return (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), issuer, audience, expiry);
    }
}
