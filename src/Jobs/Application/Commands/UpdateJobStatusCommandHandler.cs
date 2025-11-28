using AlguienDijoChamba.Api.Hubs; // Para el Hub
using AlguienDijoChamba.Api.Jobs.Domain;
using MediatR;
using Microsoft.AspNetCore.SignalR; // Para IHubContext
using AlguienDijoChamba.Api.Jobs.Domain;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AlguienDijoChamba.Api.Jobs.Application.Commands;

public class UpdateJobStatusCommandHandler(
    IJobRequestRepository jobRequestRepository,
    IHubContext<ServiceRequestHub> hubContext
) : IRequestHandler<UpdateJobStatusCommand>
{
    public async Task Handle(UpdateJobStatusCommand request, CancellationToken cancellationToken)
    {
        var jobRequest = await jobRequestRepository.GetByIdAsync(request.JobId);
    
        if (jobRequest == null)
        {
            Console.WriteLine($"❌ ERROR: Job {request.JobId} no encontrado en BD.");
            throw new Exception("Job no encontrado");
        }

        // 1. Actualizar el estado
        jobRequest.UpdateStatus(request.NewStatus);
        if(request.NewStatus == JobRequestStatus.Accepted && request.ProposedCost.HasValue)
        {
            jobRequest.UpdateCost(request.ProposedCost.Value);
        }
    
        await jobRequestRepository.UpdateAsync(jobRequest);

        // 2. Notificar
        string customerUserId = jobRequest.ClientId.ToString();
    
        // 🛑 LOGS DE DEPURACIÓN
        Console.WriteLine($"🔔 SIGNALR: Intentando notificar cambio de estado.");
        Console.WriteLine($"   - JobId: {request.JobId}");
        Console.WriteLine($"   - Nuevo Estado: {request.NewStatus}");
        Console.WriteLine($"   - Cliente ID (Destinatario): {customerUserId}");

        if (request.NewStatus == JobRequestStatus.Accepted)
        {
            await hubContext.Clients.User(customerUserId).SendAsync(
                "RequestAccepted", 
                new { 
                    JobId = request.JobId, 
                    ProfessionalId = request.ProfessionalId, 
                    ProposedCost = request.ProposedCost 
                },
                cancellationToken
            );
            Console.WriteLine($"✅ Mensaje 'RequestAccepted' enviado al usuario {customerUserId}");
        }
        else if (request.NewStatus == JobRequestStatus.Declined)
        {
            await hubContext.Clients.User(customerUserId).SendAsync(
                "RequestDeclined", 
                new { JobId = request.JobId },
                cancellationToken
            );
            Console.WriteLine($"✅ Mensaje 'RequestDeclined' enviado al usuario {customerUserId}");
        }
    }
}