using MediatR;

namespace AlguienDijoChamba.Api.Customers.Application.Queries;

public record CustomerLoginQuery(string Email, string Password) : IRequest<string>;