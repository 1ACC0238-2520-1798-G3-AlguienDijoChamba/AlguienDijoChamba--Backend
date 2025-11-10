using AlguienDijoChamba.Api.Notifications.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Notifications.Application.Commands;

public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Unit>
{
    private readonly INotificationRepository _notificationRepository;

    public MarkAsReadCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la entidad por su ID
        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification == null)
        {
            // Manejo de error si la notificación no existe (ej. usando excepciones específicas de .NET)
            throw new Exception($"Notificación con ID {request.NotificationId} no encontrada."); 
        }

        // 2. Modificar el estado usando el método de dominio
        notification.MarkAsRead(); 

        // 3. Persistir los cambios (Unit of Work)
        // Nota: Entity Framework Core rastrea los cambios de 'notification' 
        // y solo necesitas llamar a SaveChanges.
        await _notificationRepository.UnitOfWork(); 

        return Unit.Value; // Retorna Unit para indicar que la operación fue exitosa sin contenido.
    }
}