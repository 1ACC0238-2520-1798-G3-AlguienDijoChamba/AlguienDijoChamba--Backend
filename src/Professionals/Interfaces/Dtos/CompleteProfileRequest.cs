// En: AlguienDijoChamba.Api.Professionals.Interfaces.Dtos/CompleteProfileRequest.cs
namespace AlguienDijoChamba.Api.Professionals.Interfaces.Dtos;

// --- ARCHIVO ACTUALIZADO ---
public record CompleteProfileRequest(
    int YearsOfExperience,
    decimal? HourlyRate,
    string ProfessionalBio,
    string? ProfilePhotoUrl, // URL de la foto (opcional)
    List<string>? CertificationUrls // Lista de URLs de certificaciones
);