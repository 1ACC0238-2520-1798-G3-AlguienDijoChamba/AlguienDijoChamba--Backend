using MediatR;
using AlguienDijoChamba.Api.Notifications.Domain; // Para el tipo de retorno

namespace AlguienDijoChamba.Api.Notifications.Application.Queries;

/// <summary>
/// Consulta para obtener notificaciones destinadas a un Cliente específico.
/// Retorna una colección de entidades Notification.
/// </summary>
public record GetNotificationsByCustomerQuery(Guid CustomerId) : IRequest<IEnumerable<Notification>>;