using AlguienDijoChamba.Api.Notifications.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Notifications.Application.Queries;

public class GetNotificationsByProfessionalQueryHandler : IRequestHandler<GetNotificationsByProfessionalQuery, IEnumerable<Notification>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetNotificationsByProfessionalQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<Notification>> Handle(GetNotificationsByProfessionalQuery request, CancellationToken cancellationToken)
    {
        return await _notificationRepository.GetByProfessionalIdAsync(request.ProfessionalId, cancellationToken);
    }
}