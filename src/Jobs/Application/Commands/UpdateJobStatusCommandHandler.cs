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
            jobRequest.UpdateCost(request.ProposedCost.Value);
        }
    
        await jobRequestRepository.UpdateAsync(jobRequest);

        // 2. Notificar EXCLUSIVAMENTE al Cliente (Flutter)
        // Convertimos el ClientId (Guid) a string para SignalR
        string customerUserId = jobRequest.ClientId.ToString(); 
    
        if (request.NewStatus == JobRequestStatus.Accepted)
        {
            // CORRECCIÓN: Usar Clients.User para enviar solo al cliente dueño del Job
            await hubContext.Clients.User(customerUserId).SendAsync(
                "RequestAccepted", // Este nombre debe coincidir con el listener en Flutter (.on('RequestAccepted'...))
                new { // Enviamos un objeto anónimo o un DTO simple
                    request.JobId, 
                    request.ProfessionalId, 
                    request.ProposedCost 
                },
                cancellationToken
            );
        
            // Opcional: Podrías notificar a otros técnicos que el trabajo ya no está disponible
            // await hubContext.Clients.All.SendAsync("RequestTaken", request.JobId);
        }
        else if (request.NewStatus == JobRequestStatus.Declined)
        {
            // Si el técnico rechaza, notificamos al cliente (o simplemente no hacemos nada si otro técnico puede tomarlo)
            await hubContext.Clients.User(customerUserId).SendAsync(
                "RequestDeclined", 
                new { request.JobId, request.ProfessionalId },
                cancellationToken
            );
        }
    }
}