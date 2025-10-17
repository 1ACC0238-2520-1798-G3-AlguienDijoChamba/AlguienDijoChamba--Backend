namespace AlguienDijoChamba.Api.Professionals.Domain;

/// <summary>
/// Contiene la información de una persona obtenida desde RENIEC.
/// </summary>
public class ReniecInfo
{
    /// <summary>
    /// Nombres completos de la persona.
    /// </summary>
    /// <example>JULIO CESAR</example>
    public string Nombres { get; init; } = string.Empty;
    /// <summary>
    /// Apellidos completos (paterno y materno) de la persona.
    /// </summary>
    /// <example>PERALTA RAMIREZ</example>
    public string Apellidos { get; init; } = string.Empty;
}

// Interfaz para el servicio
public interface IReniecService
{
    Task<ReniecInfo?> GetReniecInfoByDni(string dni, CancellationToken cancellationToken = default);
}