namespace AlguienDijoChamba.Api.src.Professionals.Interfaces.Dtos;

// DTO para la actualización de datos del perfil (sin nombres/apellidos)
public record UpdateProfileRequest(
    string Email,
    string Celular,
    string Ocupacion,
    DateTime? FechaNacimiento,
    string? Genero
);