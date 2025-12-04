
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Commands;

public record AssignTagsToProfessionalCommand(
    Guid ProfessionalId, 
    List<Guid> TagIds 
) : IRequest<bool>;