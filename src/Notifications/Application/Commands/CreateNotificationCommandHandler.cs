using AlguienDijoChamba.Api.Notifications.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Notifications.Application.Commands;

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Guid>
{
    private readonly INotificationRepository _notificationRepository;

    public CreateNotificationCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Guid> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        // Validación básica: Debe haber al menos un destinatario
        if (request.CustomerId == Guid.Empty && request.ProfessionalId == null)
        {
            throw new ArgumentException("La notificación debe tener un CustomerId o un ProfessionalId.");
        }

        // 1. Crear la entidad de dominio
        var notification = new Notification(
            request.CustomerId,
            request.ProfessionalId,
            request.Title,
            request.Message,
            request.Type,
            request.InitialStatus
        );

        // 2. Usar el repositorio para guardar la entidad
        _notificationRepository.Add(notification);

        // 3. Persistir los cambios (Unit of Work)
        await _notificationRepository.UnitOfWork();

        // 4. Retornar el ID
        return notification.Id;
    }
}