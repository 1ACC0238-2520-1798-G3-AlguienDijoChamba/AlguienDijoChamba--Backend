using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

public record GetTagByIdQuery(Guid TagId) : IRequest<TagDto?>;