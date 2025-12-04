// En: AlguienDijoChamba.Api.Reputation.Application.Commands

using AlguienDijoChamba.Api.Reputation.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Commands;


public record CreateInitialReputationCommand(
    Guid ProfessionalId, 
    decimal InitialHourlyRate
) : IRequest<UserReputationTechnician>;