using MediatR;

namespace AlguienDijoChamba.Api.Professionals.Application.Commands;

public record UpdateProfileCommand(
    Guid UserId,
    string Email,
    string Celular,
    string Ocupacion,
    DateTime? FechaNacimiento,
    string? Genero) : IRequest<bool>;