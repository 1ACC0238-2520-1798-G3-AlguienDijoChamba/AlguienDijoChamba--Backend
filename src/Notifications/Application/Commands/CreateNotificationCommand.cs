using AlguienDijoChamba.Api.Notifications.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Notifications.Application.Commands;

/// <summary>
/// Comando para crear una nueva notificación en el sistema.
/// Implementa IRequest<Guid> para retornar el ID de la notificación creada.
/// </summary>
public record CreateNotificationCommand(
    Guid? CustomerId, 
    Guid? ProfessionalId,
    string Title,
    string Message,
    NotificationType Type,
    NotificationStatus? InitialStatus
) : IRequest<Guid>;