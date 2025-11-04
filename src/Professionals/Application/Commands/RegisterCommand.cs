using MediatR;

namespace AlguienDijoChamba.Api.src.IAM.Application.Commands;

// Define el comando con los datos que necesita y lo que devuelve (el Guid del nuevo usuario)
public record RegisterCommand(
    string Email,
    string Password,
    string Dni,
    string Nombres,
    string Apellidos,
    string Celular) : IRequest<Guid>;