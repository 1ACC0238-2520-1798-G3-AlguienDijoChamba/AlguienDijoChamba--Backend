using AlguienDijoChamba.Api.Notifications.Domain;

namespace AlguienDijoChamba.Api.Notifications.Interfaces.Dtos;

/// <summary>
/// DTO utilizado para crear una nueva notificación.
/// Mapea el cuerpo del POST /api/v1/notifications
/// </summary>
public record CreateNotificationRequestDto(
    Guid? CustomerId,
    Guid? ProfessionalId, 
    string Title,
    string Message,
    NotificationType Type,
    NotificationStatus? InitialStatus
);

