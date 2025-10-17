using AlguienDijoChamba.Api.Professionals.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Professionals.Application.Queries;

public record GetMyProfileQuery(Guid UserId) : IRequest<ProfileResponse?>;