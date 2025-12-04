using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.Notifications.Application.Commands;
using AlguienDijoChamba.Api.Notifications.Application.Queries;
using AlguienDijoChamba.Api.Notifications.Interfaces.Dtos; 
using AlguienDijoChamba.Api.Notifications.Domain;
using AlguienDijoChamba.Api.Professionals.Domain; // Necesario para los Enums en los DTOs de Request

using MediatR; 
using Microsoft.AspNetCore.Mvc;

namespace AlguienDijoChamba.Api.Notifications.Interfaces;

[ApiController]
[Route("api/v1/notifications")] // Base: /api/v1/notifications
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProfessionalRepository _professionalRepository; 
    private readonly ICustomerRepository _customerRepository; 

    public NotificationsController(
        IMediator mediator, 
        IProfessionalRepository professionalRepository, // Asume inyección
        ICustomerRepository customerRepository)         // Asume inyección
    {
        _mediator = mediator;
        _professionalRepository = professionalRepository;
        _customerRepository = customerRepository;
    }
    

    // --- 1. POST: Crear una nueva notificación ---
    // POST /api/v1/notifications
    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequestDto requestDto)
    {
        // Usamos los tipos del DTO que ahora son Enums (gracias al Model Binding)
        var command = new CreateNotificationCommand(
            requestDto.CustomerId,
            requestDto.ProfessionalId,
            requestDto.Title,
            requestDto.Message,
            requestDto.Type,            // Usamos el Enum directamente
            requestDto.InitialStatus    // Usamos el Enum? directamente
        );

        var notificationId = await _mediator.Send(command);
        
        // Retorna 201 Created con la ubicación del nuevo recurso
        return CreatedAtAction(nameof(GetNotificationById), new { id = notificationId }, notificationId);
    }
    
    // Método auxiliar (Simulado para que funcione CreatedAtAction)
    [HttpGet("{id:guid}", Name = "GetNotificationById")]
    public Task<IActionResult> GetNotificationById(Guid id)
    {
        // En un sistema real, esto enviaría un GetNotificationByIdQuery.
        return Task.FromResult<IActionResult>(Ok(new { Id = id, Status = "Pending" }));
    }

// --- 2. GET: Obtener notificaciones del Cliente (Customer) ---
// GET /api/v1/notifications/customers/{customerId}
[HttpGet("customers/{customerId:guid}")]
public async Task<IActionResult> GetNotificationsByCustomer([FromRoute] Guid customerId)
{
    var query = new GetNotificationsByCustomerQuery(customerId);
    var notifications = await _mediator.Send(query);

    var response = new List<NotificationResponseDto>();

    // 🛑 Iteramos para construir la respuesta
    foreach (var n in notifications)
    {
        string senderName = "Administrador del Sistema"; 

        // 🛑 Lógica: Si la notificación tiene ProfessionalId, buscamos ese perfil.
        if (n.ProfessionalId.HasValue) 
        {
            try
            {
                // La búsqueda del Profesional remitente
                var professional = await _professionalRepository.GetByIdAsync(n.ProfessionalId.Value, CancellationToken.None);
                
                // Usamos professional.Nombres si existe, sino "Usuario Eliminado"
                senderName = professional != null ? professional.Nombres : "Usuario Eliminado";
            }
            catch (Exception)
            {
                // Capturamos cualquier excepción de "No encontrado" y damos un nombre seguro.
                senderName = "Usuario Eliminado/No Encontrado";
            }
        } 
    
        // Mapeo final
        response.Add(new NotificationResponseDto(
            n.Id, 
            n.Title, 
            n.Message, 
            n.Type.ToString(),
            n.Status?.ToString(),
            n.IsRead, 
            n.CreatedAt,
            n.ProfessionalId,
            n.CustomerId,
            SenderName: senderName // 🛑 Campo SenderName añadido
        ));
    }

    return Ok(response);
}

// --- 3. GET: Obtener notificaciones del Profesional (Professional) ---
// GET /api/v1/notifications/professionals/{professionalId}
[HttpGet("professionals/{professionalId:guid}")]
public async Task<IActionResult> GetNotificationsByProfessional([FromRoute] Guid professionalId)
{
    var query = new GetNotificationsByProfessionalQuery(professionalId); 
    var notifications = await _mediator.Send(query);

    var response = new List<NotificationResponseDto>();

    // 🛑 Iteramos para construir la respuesta
    foreach (var n in notifications)
    {
        string senderName = "Administrador del Sistema"; 

        // 🛑 Lógica: Si la notificación tiene CustomerId, buscamos ese perfil (el remitente).
        if (n.CustomerId.HasValue) 
        {
            try
            {
                // Buscamos el perfil del Cliente remitente
                var customer = await _customerRepository.GetByUserIdAsync(n.CustomerId.Value, CancellationToken.None);                // Usamos customer.Nombres si existe, sino "Usuario Eliminado"
                senderName = customer != null ? customer.Nombres : "Usuario Eliminado";

            }
            catch (Exception)
            {
                // Capturamos cualquier excepción de "No encontrado"
                senderName = "Usuario Eliminado/No Encontrado";
            }
        }
    
        // Mapeo final
        response.Add(new NotificationResponseDto(
            n.Id, 
            n.Title, 
            n.Message, 
            n.Type.ToString(),
            n.Status?.ToString(),
            n.IsRead, 
            n.CreatedAt,
            n.ProfessionalId,
            n.CustomerId,
            SenderName: senderName // 🛑 Campo SenderName añadido
        ));
    }

    return Ok(response);
}

    // --- 4. PATCH: Marcar como leída ---
    // PATCH /api/v1/notifications/{id}/mark-as-read
    [HttpPatch("{id:guid}/mark-as-read")]
    public async Task<IActionResult> MarkAsRead([FromRoute] Guid id)
    {
        var command = new MarkAsReadCommand(id);
        await _mediator.Send(command);
        return NoContent(); // 204 No Content
    }
    
    // --- 5. DELETE: Descartar (Soft Delete) una Notificación ---
    // DELETE /api/v1/notifications/{id}
    [HttpDelete("{id:guid}")] 
    public async Task<IActionResult> DismissNotification([FromRoute] Guid id)
    {
        // Este comando actualiza el Status a Discarded en el Repositorio
        var command = new DismissNotificationCommand(id); 
        await _mediator.Send(command);
        
        // Retorna 204 No Content para indicar que el recurso se ha "eliminado" (descartado)
        return NoContent(); 
    }
}