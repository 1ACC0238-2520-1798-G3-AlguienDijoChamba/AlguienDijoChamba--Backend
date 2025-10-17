using MediatR;

namespace AlguienDijoChamba.Api.IAM.Application.Queries;

public record LoginQuery(string Email, string Password) : IRequest<string>; // <-- Devuelve el token