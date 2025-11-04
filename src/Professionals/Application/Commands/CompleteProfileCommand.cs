// En: AlguienDijoChamba.Api.Professionals.Application.Commands/CompleteProfileCommand.cs
using MediatR;

namespace AlguienDijoChamba.Api.Professionals.Application.Commands;

// --- ARCHIVO ACTUALIZADO ---
public record CompleteProfileCommand(
    Guid UserId,
    int YearsOfExperience,
    decimal? HourlyRate,
    string ProfessionalBio,
    string? ProfilePhotoUrl,
    List<string>? CertificationUrls) : IRequest<bool>;