using Microsoft.Extensions.Configuration;
using TechLogistics.Web.Auth;
using Xunit;

namespace TechLogistics.Tests.Auth;

public class JwtTokenServiceTests
{
    private static JwtTokenService CrearSut()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "clave-de-pruebas-unitarias-min-32-caracteres!!",
                ["Jwt:Issuer"] = "TechLogisticsTests",
                ["Jwt:Audience"] = "TechLogisticsTestsAudience",
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();

        return new JwtTokenService(config);
    }

    [Fact]
    public void GenerarToken_ProduceUnTokenNoVacio()
    {
        var sut = CrearSut();
        var token = sut.GenerarToken("gerente1", "GerenteBodega");
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void ValidarToken_ConTokenValido_DevuelvePrincipalConClaimsCorrectos()
    {
        var sut = CrearSut();
        var token = sut.GenerarToken("agente1", "AgenteCampo");

        var principal = sut.ValidarToken(token);

        Assert.NotNull(principal);
        Assert.Equal("agente1", principal!.Identity!.Name);
        Assert.Contains(principal.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "AgenteCampo");
    }

    [Fact]
    public void ValidarToken_ConTokenManipulado_DevuelveNull()
    {
        var sut = CrearSut();
        var token = sut.GenerarToken("gerente1", "GerenteBodega");
        var tokenManipulado = token[..^5] + "AAAAA";

        var principal = sut.ValidarToken(tokenManipulado);

        Assert.Null(principal);
    }

    [Fact]
    public void ValidarToken_ConTokenDeOtroIssuer_DevuelveNull()
    {
        var configOtroIssuer = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "clave-de-pruebas-unitarias-min-32-caracteres!!",
                ["Jwt:Issuer"] = "OtroEmisor",
                ["Jwt:Audience"] = "TechLogisticsTestsAudience",
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();
        var otroEmisor = new JwtTokenService(configOtroIssuer);
        var token = otroEmisor.GenerarToken("intruso", "GerenteBodega");

        var sut = CrearSut();
        var principal = sut.ValidarToken(token);

        Assert.Null(principal);
    }
}
