using AlguienDijoChamba.Api.Jobs.Application.Commands;
using AlguienDijoChamba.Api.Jobs.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AlguienDijoChamba.Api.Hubs;

[Authorize] // Requiere Token
public class ServiceRequestHub : Hub
{
    private readonly ISender _sender;

    public ServiceRequestHub(ISender sender)
    {
        _sender = sender;
    }

    public override async Task OnConnectedAsync()
    {
        // Debug: Ver quién se conectó
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine($"✅ SIGNALR: Cliente conectado. ConnectionId: {Context.ConnectionId}, UserId: {userId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"❌ SIGNALR: Cliente desconectado. ConnectionId: {Context.ConnectionId}. Error: {exception?.Message}");
        await base.OnDisconnectedAsync(exception);
    }

    // Método llamado por el TÉCNICO (Android) para aceptar/rechazar
    public async Task RespondToRequest(Guid jobId, bool accepted, decimal proposedCost)
    {
        try 
        {
            var professionalIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"📩 SIGNALR: RespondToRequest recibido. JobId: {jobId}, Accepted: {accepted}, ProfId: {professionalIdStr}");

            if (string.IsNullOrEmpty(professionalIdStr) || !Guid.TryParse(professionalIdStr, out var professionalId))
            {
                throw new HubException("No se pudo identificar al profesional.");
            }

            var newStatus = accepted ? JobRequestStatus.Accepted : JobRequestStatus.Declined;

            var command = new UpdateJobStatusCommand(
                jobId, 
                newStatus, 
                professionalId, 
                proposedCost
            );
            
            await _sender.Send(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en RespondToRequest: {ex.Message}");
            throw;
        }
    }
}