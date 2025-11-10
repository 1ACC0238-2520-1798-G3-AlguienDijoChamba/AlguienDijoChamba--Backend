namespace AlguienDijoChamba.Api.Notifications.Interfaces.Dtos;

/// <summary>
/// DTO utilizado para devolver la información de una notificación al cliente.
/// Mapea la respuesta de los GET /customers/{id} y /professionals/{id}
/// </summary>
public record NotificationResponseDto(
    Guid Id,
    string Title,
    string Message,
    string Type,
    string? Status,
    bool IsRead,
    DateTime CreatedAt,

    Guid? ProfesionalId,
    Guid? CustomerId,
    string SenderName
);
