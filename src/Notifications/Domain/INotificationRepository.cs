namespace AlguienDijoChamba.Api.Notifications.Domain;

public interface INotificationRepository
{
    void Add(Notification notification);
    Task<IEnumerable<Notification>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken = default);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(Notification notification);
    Task<int> UnitOfWork(); // Si usas un patrón Unit of Work
    void Remove(Notification notification);
}