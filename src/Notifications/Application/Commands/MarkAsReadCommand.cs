using MediatR;

namespace AlguienDijoChamba.Api.Notifications.Application.Commands;

/// <summary>
/// Comando para marcar una notificación específica como leída.
/// Implementa IRequest<Unit> porque no retorna contenido (el endpoint retorna 204 No Content).
/// </summary>
public record MarkAsReadCommand(Guid NotificationId) : IRequest<Unit>;