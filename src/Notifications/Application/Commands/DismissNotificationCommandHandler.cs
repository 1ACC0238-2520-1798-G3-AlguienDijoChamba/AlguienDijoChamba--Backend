using AlguienDijoChamba.Api.Notifications.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Notifications.Application.Commands;

public class DismissNotificationCommandHandler : IRequestHandler<DismissNotificationCommand, Unit>
{
    private readonly INotificationRepository _notificationRepository;

    public DismissNotificationCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Unit> Handle(DismissNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
    
        if (notification == null) return Unit.Value;

        // Llama al método de dominio para cambiar el Status a Discarded
        notification.Discard(); 

        await _notificationRepository.UnitOfWork(); 
        return Unit.Value;
    }
}