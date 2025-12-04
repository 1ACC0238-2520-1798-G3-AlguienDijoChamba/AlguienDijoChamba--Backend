using MediatR;
using System.Collections.Generic;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

// Esta Query no necesita argumentos y retorna una colección de DTOs.
public record GetAllReputationsQuery : IRequest<IEnumerable<ReputationDto>>;