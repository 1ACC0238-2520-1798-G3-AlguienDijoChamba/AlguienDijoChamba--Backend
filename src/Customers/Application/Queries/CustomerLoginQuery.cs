using AlguienDijoChamba.Api.Customers.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Customers.Application.Queries;

public record CustomerLoginQuery(string Email, string Password) : IRequest<LoginResponseDto>;
