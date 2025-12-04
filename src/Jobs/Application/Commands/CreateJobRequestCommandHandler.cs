using AlguienDijoChamba.Api.Hubs;
using AlguienDijoChamba.Api.Jobs.Domain;
using AlguienDijoChamba.Api.Jobs.Interfaces.Dtos;
using AlguienDijoChamba.Api.Professionals.Domain; 
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace AlguienDijoChamba.Api.Jobs.Application.Commands;

public class CreateJobRequestCommandHandler(
    IJobRequestRepository jobRequestRepository,
    IProfessionalRepository professionalRepository,
    IUnitOfWork unitOfWork,
    IHubContext<ServiceRequestHub> hubContext
) : IRequestHandler<CreateJobRequestCommand, JobDto>
{
    public async Task<JobDto> Handle(CreateJobRequestCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[CMD] Iniciando creación de Job. ProfessionalId: {request.ProfessionalId}");

        // 1. Crear la entidad (Estado Pending)
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

        // 2. Guardar en BD
        try 
        {
            jobRequestRepository.Add(jobRequest);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"[CMD] Job guardado en BD con ID: {jobRequest.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Falló al guardar en BD: {ex.Message}");
            throw; // Re-lanzar para que falle la petición HTTP
        }

        // 3. Mapear DTO
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

        // 4. Notificación SignalR (BLINDADA)
        try
        {
            // Buscar el perfil profesional
            var professionalEntity = await professionalRepository.GetByIdAsync(request.ProfessionalId, cancellationToken);

            if (professionalEntity != null)
            {
                var targetSignalRUserId = professionalEntity.UserId.ToString();
                Console.WriteLine($"📣 SIGNALR: Intentando notificar a UserId: {targetSignalRUserId}");

                // Enviar mensaje
                await hubContext.Clients.User(targetSignalRUserId)
                    .SendAsync("ReceiveNewRequest", jobDto, cancellationToken);
                
                Console.WriteLine($"📣 SIGNALR: Mensaje enviado (o encolado) a {targetSignalRUserId}");
            }
            else
            {
                Console.WriteLine($"⚠️ SIGNALR ERROR: No se encontró Professional con ID {request.ProfessionalId}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ SIGNALR EXCEPTION: {ex.Message}");
            // No lanzamos throw aquí para no fallar la respuesta HTTP si solo falla el socket
        }

        return jobDto;
    }
}