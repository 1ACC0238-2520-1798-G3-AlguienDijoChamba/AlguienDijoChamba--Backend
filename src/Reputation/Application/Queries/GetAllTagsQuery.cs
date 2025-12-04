using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;

// Usamos el DTO de salida

// No necesita parámetros, solo solicita la lista
namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

public record GetAllTagsQuery : IRequest<IEnumerable<TagDto>>;