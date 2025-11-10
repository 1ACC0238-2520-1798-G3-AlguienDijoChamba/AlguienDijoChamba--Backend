using MediatR;

namespace AlguienDijoChamba.Api.Notifications.Application.Commands;

/// <summary>
/// Comando para marcar una notificación como descartada/eliminada por el usuario.
/// </summary>
public record DismissNotificationCommand(Guid NotificationId) : IRequest<Unit>;