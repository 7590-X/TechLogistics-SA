namespace TechLogistics.Web.Auth;

public class DemoUserStore
{
    private readonly record struct DemoUser(string Usuario, string Password, string Rol);

    private static readonly DemoUser[] Usuarios =
    {
        new("gerente1", "Gerente#2026", "GerenteBodega"),
        new("agente1", "Agente#2026", "AgenteCampo")
    };

    public string? ValidarCredenciales(string usuario, string password)
    {
        var encontrado = Usuarios.FirstOrDefault(u =>
            string.Equals(u.Usuario, usuario, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

        return encontrado.Usuario is null ? null : encontrado.Rol;
    }
}
