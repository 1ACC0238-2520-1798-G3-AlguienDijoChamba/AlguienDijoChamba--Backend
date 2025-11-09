using MediatR;
using AlguienDijoChamba.Api.Notifications.Domain;

namespace AlguienDijoChamba.Api.Notifications.Application.Queries;

/// <summary>
/// Consulta para obtener notificaciones destinadas a un Profesional específico.
/// Retorna una colección de entidades Notification.
/// </summary>
public record GetNotificationsByProfessionalQuery(Guid ProfessionalId) : IRequest<IEnumerable<Notification>>;