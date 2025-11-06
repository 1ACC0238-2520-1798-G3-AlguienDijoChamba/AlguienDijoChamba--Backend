using AlguienDijoChamba.Api.Reputation.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Commands;

public record RecalculateReputationCommand(
    Guid ProfessionalId, 
    double NewRatingValue
) : IRequest<UserReputationTechnician>;