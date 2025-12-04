namespace AlguienDijoChamba.Api.Notifications.Domain;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid? ProfessionalId { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationStatus? Status { get; private set; }
    public bool IsRead { get; private set; } = false;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Constructor privado para EF Core o reconstrucción
    private Notification() {}

    // Constructor para la creación
    public Notification(Guid? customerId, Guid? professionalId, string title, string message, NotificationType type, NotificationStatus? status)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        ProfessionalId = professionalId;
        Title = title;
        Message = message;
        Type = type;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
    public void UpdateStatus(NotificationStatus newStatus)
    {
        if (Status.HasValue) // Solo actualizar si el campo tiene un valor inicial
        {
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }
    }
    public void Discard()
    {
        Status = NotificationStatus.Discarded;
        UpdatedAt = DateTime.UtcNow;
    }
}