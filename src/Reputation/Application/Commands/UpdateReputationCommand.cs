using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Commands;

public record UpdateReputationCommand(
    Guid ProfessionalId,
    double Rating,
    int ReviewsCount,
    string Level,
    decimal HourlyRate
) : IRequest<Domain.UserReputationTechnician>;