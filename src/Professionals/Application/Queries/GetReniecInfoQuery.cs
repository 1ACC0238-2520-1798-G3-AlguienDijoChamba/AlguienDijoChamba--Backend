using AlguienDijoChamba.Api.Professionals.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Professionals.Application.Queries;

public record GetReniecInfoQuery(string Dni) : IRequest<ReniecInfo?>;