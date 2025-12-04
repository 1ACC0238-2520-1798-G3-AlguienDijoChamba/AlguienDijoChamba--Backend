namespace AlguienDijoChamba.Api.IAM.Interfaces.Dtos;

/// <summary>
/// Respuesta del endpoint de login.
/// ✅ Incluye token Y userId para que el cliente pueda identificar al usuario.
/// </summary>
public record LoginResponse(string Token, string? UserId = null);
