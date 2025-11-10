using AlguienDijoChamba.Api.Notifications.Domain;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;
using Microsoft.EntityFrameworkCore;

namespace AlguienDijoChamba.Api.Notifications.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Notification notification) => _context.Notifications.Add(notification);

    public async Task<IEnumerable<Notification>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default) =>
        await _context.Notifications
            .Where(n => n.CustomerId == customerId && n.Status != NotificationStatus.Discarded)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    
    public async Task<IEnumerable<Notification>> GetByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken = default) =>
        await _context.Notifications
            .Where(n => n.ProfessionalId == professionalId && n.Status != NotificationStatus.Discarded)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    
    public void Update(Notification notification) => _context.Notifications.Update(notification);

    public async Task<int> UnitOfWork() => await _context.SaveChangesAsync();
    public void Remove(Notification notification) => _context.Notifications.Remove(notification);
    
}