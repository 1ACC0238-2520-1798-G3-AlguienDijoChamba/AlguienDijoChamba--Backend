using MediatR;

namespace AlguienDijoChamba.Api.Professionals.Application.Commands;

public record CompleteProfileCommand(
    Guid UserId,
    int YearsOfExperience,
    decimal? HourlyRate,
    string ProfessionalBio) : IRequest<bool>;