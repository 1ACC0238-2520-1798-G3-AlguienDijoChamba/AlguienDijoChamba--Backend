using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

public record GetReputationByProfessionalIdQuery(Guid ProfessionalId) : IRequest<ReputationResponse?>;
