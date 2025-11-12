// En: Hubs/ServiceRequestHub.cs
using AlguienDijoChamba.Api.Jobs.Application.Commands;
using AlguienDijoChamba.Api.Jobs.Domain;
using AlguienDijoChamba.Api.Jobs.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AlguienDijoChamba.Api.Hubs;

[Authorize] // Solo usuarios autenticados (Técnicos) pueden conectarse al Hub
public class ServiceRequestHub : Hub
{
    private readonly ISender _sender;
    public ServiceRequestHub(ISender sender) { _sender = sender; }

    // --- Método llamado por el TÉCNICO (Android) ---
    // Este método se llamará desde el frontend del Técnico
    public async Task RespondToRequest(Guid jobId, bool accepted, decimal proposedCost)
    {
        var professionalIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(professionalIdStr) || !Guid.TryParse(professionalIdStr, out var professionalId))
        {
            throw new HubException("No se pudo identificar al profesional.");
        }

        var newStatus = accepted ? JobRequestStatus.Accepted : JobRequestStatus.Declined;

        // 3. Enviamos un comando a MediatR para que actualice la BD
        // 🚀 NUEVO: Pasamos el costo propuesto al comando
        var command = new UpdateJobStatusCommand(
            jobId, 
            newStatus, 
            professionalId, 
            proposedCost // <-- El nuevo costo
        );
        
        await _sender.Send(command);
    }
    
    // (Opcional) Unir usuarios a grupos por especialidad al conectarse
    public override async Task OnConnectedAsync()
    {
        // var userId = Context.UserIdentifier; // ID del usuario (GUID)
        // Lógica para unir a grupos (ej. "plomeros", "electricistas")
        await base.OnConnectedAsync();
    }
}