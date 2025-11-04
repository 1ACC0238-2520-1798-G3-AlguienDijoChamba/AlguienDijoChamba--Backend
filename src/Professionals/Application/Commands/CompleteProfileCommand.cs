using MediatR;
using System.Collections.Generic; // Asegúrate de que esta importación exista

namespace AlguienDijoChamba.Api.Professionals.Application.Commands;

public record CompleteProfileCommand(
    Guid UserId,
    int YearsOfExperience,
    decimal? HourlyRate,
    string ProfessionalBio,
    string? ProfilePhotoUrl,          // <-- ¡AÑADE ESTO!
    List<string>? CertificationUrls   // <-- ¡AÑADE ESTO!
) : IRequest<bool>;