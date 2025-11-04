namespace AlguienDijoChamba.Api.Professionals.Interfaces.Dtos;

public record ProfileResponse(
    string UserName,
    string ProfessionalLevel,
    double StarRating,
    int CompletedJobs,
    decimal AvailableBalance,
    // --- NUEVOS CAMPOS ---
    string Nombres,
    string Apellidos,
    string Ocupacion,
    string Email,
    string Celular,
    DateTime? FechaNacimiento,
    string? Genero,
    string? FotoPerfilUrl
);