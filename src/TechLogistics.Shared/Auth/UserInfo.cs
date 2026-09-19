namespace TechLogistics.Shared.Auth;

/// <summary>
/// Snapshot serializable del usuario autenticado, persistido por el servidor al final
/// del renderizado estático (PersistentComponentState) y leído por el cliente WASM al
/// arrancar, evitando una segunda validación de credenciales en el navegador.
/// Debe vivir en Shared: el mismo tipo se serializa en el servidor y se deserializa
/// en el cliente WebAssembly.
/// </summary>
public record UserInfo(string UserName, string[] Roles);
