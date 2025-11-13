using AlguienDijoChamba.Api.Hubs;
using AlguienDijoChamba.Api.Jobs.Domain;
using AlguienDijoChamba.Api.Jobs.Interfaces.Dtos;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace AlguienDijoChamba.Api.Jobs.Application.Commands;

public class CreateJobRequestCommandHandler(
    IJobRequestRepository jobRequestRepository,
    IUnitOfWork unitOfWork,
    IHubContext<ServiceRequestHub> hubContext
) : IRequestHandler<CreateJobRequestCommand, JobDto>
{
    public async Task<JobDto> Handle(CreateJobRequestCommand request, CancellationToken cancellationToken)
    {
        // 1. Crear la entidad JobRequest (Estado PENDING por defecto, no Accepted)
        // NOTA: Asegúrate de que 'CreateActiveJob' en tu dominio cree el estado como 'Pending' si quieres flujo real.
        // Si usas tu lógica actual 'CreateActiveJob' que pone 'Accepted', corrígelo si deseas flujo de negociación.
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

        jobRequestRepository.Add(jobRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var jobDto = new JobDto
        {
            Id = jobRequest.Id,
            ClientId = jobRequest.ClientId,
            // ... resto de mapeo ...
            Specialty = jobRequest.Specialty,
            Description = jobRequest.Description,
            Address = jobRequest.Address,
            TotalCost = jobRequest.TotalCost,
            Status = jobRequest.Status.ToString(),
            CreatedAt = jobRequest.CreatedAt
        };

        // 🚀 NOTIFICAR A TÉCNICOS
        Console.WriteLine($"📣 SIGNALR: Enviando 'ReceiveNewRequest' para Job {jobDto.Id}");
        
        // Envía a TODOS. Para producción, deberías filtrar por Grupos o IDs.
        await hubContext.Clients.All.SendAsync("ReceiveNewRequest", jobDto, cancellationToken);
        
        return jobDto;
    }
}