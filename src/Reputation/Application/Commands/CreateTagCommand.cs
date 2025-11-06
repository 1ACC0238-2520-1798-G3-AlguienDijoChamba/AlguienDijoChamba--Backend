
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Commands;

public record CreateTagCommand(string Name) : IRequest<TagDto>;