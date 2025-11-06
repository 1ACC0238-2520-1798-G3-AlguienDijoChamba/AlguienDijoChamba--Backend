using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

public record GetTagsByProfessionalIdQuery(Guid ProfessionalId) : IRequest<IEnumerable<TagDto>>;