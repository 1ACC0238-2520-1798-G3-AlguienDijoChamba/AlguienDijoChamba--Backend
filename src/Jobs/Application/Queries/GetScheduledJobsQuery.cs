using AlguienDijoChamba.Api.Jobs.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Jobs.Application.Queries;

public record GetScheduledJobsQuery(Guid UserId) : IRequest<IEnumerable<JobDto>>;