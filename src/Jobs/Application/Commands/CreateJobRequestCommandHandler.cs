using AlguienDijoChamba.Api.Hubs; // Para el Hub
using AlguienDijoChamba.Api.Jobs.Domain;
using AlguienDijoChamba.Api.Jobs.Interfaces.Dtos;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.SignalR; // Para IHubContext
using System.Threading;
using System.Threading.Tasks;

namespace AlguienDijoChamba.Api.Jobs.Application.Commands;

public class CreateJobRequestCommandHandler(
    IJobRequestRepository jobRequestRepository,
    IUnitOfWork unitOfWork,
    IHubContext<ServiceRequestHub> hubContext // <-- Inyectamos el Hub
) : IRequestHandler<CreateJobRequestCommand, JobDto>
{
    public async Task<JobDto> Handle(CreateJobRequestCommand request, CancellationToken cancellationToken)
    {
        // 1. Crear la entidad JobRequest (basado en tu CreateActiveJob)
        var jobRequest = JobRequest.CreateActiveJob(
            clientId: request.CustomerId,
            professionalId: request.ProfessionalId,
            specialty: request.Specialty,
            description: request.Description,
            address: request.Address,
            scheduledDate: request.ScheduledDate,
            scheduledHour: request.ScheduledHour,
            additionalMessage: request.AdditionalMessage,
            categories: request.Categories,
            paymentMethod: request.PaymentMethod,
            totalCost: request.TotalCost
        );
        
        // 2. Guardar en la Base de Datos
        jobRequestRepository.Add(jobRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 3. Mapear a DTO para enviar por SignalR
        var jobDto = new JobDto
        {
            Id = jobRequest.Id,
            ClientId = jobRequest.ClientId,
            ProfessionalId = jobRequest.ProfessionalId ?? Guid.Empty,
            Specialty = jobRequest.Specialty,
            Description = jobRequest.Description,
            Address = jobRequest.Address,
            ScheduledDate = jobRequest.ScheduledDate,
            ScheduledHour = jobRequest.ScheduledHour,
            AdditionalMessage = jobRequest.AdditionalMessage,
            Categories = jobRequest.Categories,
            PaymentMethod = jobRequest.PaymentMethod,
            TotalCost = jobRequest.TotalCost,
            Status = jobRequest.Status.ToString(),
            CreatedAt = jobRequest.CreatedAt,
            UpdatedAt = jobRequest.UpdatedAt
        };

        // 4. 🚀 ENVIAR NOTIFICACIÓN AL HUB (Técnicos)
        // Notifica a todos los clientes conectados a este Hub (los Técnicos)
        // que ha llegado una nueva solicitud.
        await hubContext.Clients.All.SendAsync("ReceiveNewRequest", jobDto, cancellationToken);
        
        // 5. Devolver el DTO al Cliente (Flutter) que hizo la llamada REST
        return jobDto;
    }
}