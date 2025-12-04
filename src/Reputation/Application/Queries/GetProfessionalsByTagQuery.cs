// Archivo: GetProfessionalsByTagQuery.cs

using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

// Asume que IRequest pertenece a MediatR
public record GetProfessionalsByTagQuery(Guid TagId) : IRequest<IEnumerable<Guid>>;