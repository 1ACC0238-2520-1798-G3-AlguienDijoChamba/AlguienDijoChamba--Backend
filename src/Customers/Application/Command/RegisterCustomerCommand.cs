using MediatR;

namespace AlguienDijoChamba.Api.Customers.Application.Command;

public record RegisterCustomerCommand(
    string Email,
    string Password,
    string Nombres,
    string Apellidos,
    string Celular
) : IRequest<Guid>;