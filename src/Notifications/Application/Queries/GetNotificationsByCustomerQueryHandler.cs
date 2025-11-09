using AlguienDijoChamba.Api.Notifications.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Notifications.Application.Queries;

public class GetNotificationsByCustomerQueryHandler : IRequestHandler<GetNotificationsByCustomerQuery, IEnumerable<Notification>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetNotificationsByCustomerQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<Notification>> Handle(GetNotificationsByCustomerQuery request, CancellationToken cancellationToken)
    {
        // Llama al método del repositorio que filtra por CustomerId
        return await _notificationRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
    }
}