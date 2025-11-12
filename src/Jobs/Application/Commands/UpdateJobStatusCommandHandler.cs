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
            throw new Exception("Job no encontrado");

        // 1. Actualizar el estado en la BD
        jobRequest.UpdateStatus(request.NewStatus);
        
        if(request.NewStatus == JobRequestStatus.Accepted && request.ProposedCost.HasValue)
        {
            jobRequest.UpdateCost(request.ProposedCost.Value); // Necesitarás crear este método en JobRequest.cs
        }
        
        // (Tu implementación de UpdateAsync ya guarda cambios, si no, necesitas IUnitOfWork aquí)
        await jobRequestRepository.UpdateAsync(jobRequest); 
        
        string customerClientId = jobRequest.ClientId.ToString(); 
        string eventName;
        
        if (request.NewStatus == JobRequestStatus.Accepted)
        {
            eventName = "RequestAccepted";
            await hubContext.Clients.All.SendAsync( // (O Clients.User(customerClientId))
                eventName, 
                request.JobId, 
                request.ProfessionalId, 
                request.ProposedCost, // <-- 🚀 ENVIAR EL COSTO AL CLIENTE
                cancellationToken
            );
            
            // Notificar a OTROS técnicos que este job ya fue tomado
            // (Deberíamos obtener la ConnectionId del técnico que aceptó)
            await hubContext.Clients.All.SendAsync("RequestTaken", request.JobId, cancellationToken);
        }
        else if (request.NewStatus == JobRequestStatus.Declined)
        {
            eventName = "RequestDeclined";
            await hubContext.Clients.All.SendAsync(
                eventName, 
                request.JobId, 
                request.ProfessionalId, 
                cancellationToken
            );
        }
    }
}