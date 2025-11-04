using MediatR;
using System;

namespace AlguienDijoChamba.Api.IAM.Application.Commands;

// El comando solo necesita el ID del usuario, que se obtiene del token.
public record DeleteAccountCommand(Guid UserId) : IRequest<bool>;