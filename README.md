# TechLogistics S.A. — Portal Interactivo de Gestión Logística

Sistema de gestión logística de inventarios en tiempo real, construido sobre
**ASP.NET Core Blazor Web App** con Render Modes intercalados.

## Estructura del repositorio

```
TechLogistics.sln
protos/telemetry.proto                Contrato gRPC compartido
src/
  TechLogistics.Web/                  Host de la Blazor Web App
  TechLogistics.Client/               Proyecto Blazor WebAssembly
  TechLogistics.Components/           Razor Class Library — 3 componentes reutilizables
  TechLogistics.Shared/               Modelos, enums, cliente REST, UserInfo
  TechLogistics.TelemetryService/     Servidor gRPC de telemetría (streaming)
tests/
  TechLogistics.Tests/                Pruebas xUnit y bUnit
```

## Requisitos previos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 (17.9+) o VS Code con C# Dev Kit

## Cómo compilar y ejecutar localmente

1. Clonar/descomprimir el repositorio.
2. `dotnet restore`
3. `dotnet build`
4. (Opcional) Ejecutar el servicio de telemetría en una terminal aparte:
   ```bash
   dotnet run --project src/TechLogistics.TelemetryService/TechLogistics.TelemetryService.csproj
   ```
5. Ejecutar el host principal:
   ```bash
   dotnet run --project src/TechLogistics.Web/TechLogistics.Web.csproj
   ```
6. Rutas: `/` (inicio), `/login`, `/bodega/monitoreo` (GerenteBodega), `/despacho/escaneo` (AgenteCampo).

### Usuarios de demostración

| Usuario    | Contraseña      | Rol            |
|------------|------------------|----------------|
| `gerente1` | `Gerente#2026`   | GerenteBodega  |
| `agente1`  | `Agente#2026`    | AgenteCampo    |

## Cómo ejecutar las pruebas

```bash
dotnet test tests/TechLogistics.Tests/TechLogistics.Tests.csproj --collect:"XPlat Code Coverage"
```

## Notas de compilación — correcciones aplicadas

Al compilar por primera vez en Visual Studio aparecieron 4 errores de paquetes NuGet
faltantes, ya corregidos en esta versión (detalle completo en `log.txt`):

1. **`TechLogistics.Components.csproj`** necesita `Microsoft.AspNetCore.Components.Web`
   (una Razor Class Library no trae por sí sola `Parameter`, `RenderFragment`,
   `EventCallback`, `EditForm`, etc.).
2. **`TechLogistics.Client.csproj`** necesita `Microsoft.Extensions.Http`
   (requerido por `AddHttpClient<TClient, TImplementation>`).
3. **`TechLogistics.Client/_Imports.razor`** necesita
   `@using static Microsoft.AspNetCore.Components.Web.RenderMode` (para usar
   `InteractiveWebAssembly` sin prefijo en `@rendermode`).
4. **`TechLogistics.Web.csproj`** necesita `Microsoft.AspNetCore.Components.WebAssembly.Server`
   (trae `AddInteractiveWebAssemblyComponents()` y `AddInteractiveWebAssemblyRenderMode()`,
   que no forman parte del framework compartido de ASP.NET Core).

**Importante:** este repositorio fue construido y revisado por inspección de código —
no pude ejecutar `dotnet build` en el entorno donde lo generé (sin acceso a NuGet.org).
Se hizo una auditoría manual de cada API contra su paquete NuGet real para los 6 proyectos,
pero **verifica con `dotnet restore && dotnet build` antes de la entrega final**, y avísame
si aparece algún error adicional.
