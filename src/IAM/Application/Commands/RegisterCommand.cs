using MediatR;

namespace AlguienDijoChamba.Api.IAM.Application.Commands;

public record RegisterCommand(
    string Email,
    string Password,
    string Dni,
    string Nombres,
    string Apellidos,
    string Celular) : IRequest<Guid>; // <-- Devuelve el ID del usuario