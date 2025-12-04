using AlguienDijoChamba.Api.Jobs.Domain;
using AlguienDijoChamba.Api.Jobs.Interfaces.Dtos;
using AlguienDijoChamba.Api.Professionals.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Jobs.Application.Queries;

public class GetScheduledJobsQueryHandler(
    IJobRequestRepository jobRepository,
    IProfessionalRepository professionalRepository
) : IRequestHandler<GetScheduledJobsQuery, IEnumerable<JobDto>>
{
    public async Task<IEnumerable<JobDto>> Handle(GetScheduledJobsQuery request, CancellationToken cancellationToken)
    {
        // 1. Obtener el perfil profesional asociado al usuario logueado
        var professional = await professionalRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        if (professional is null) return [];

        // 2. Obtener los trabajos aceptados
        var jobs = await jobRepository.GetAcceptedJobsByProfessionalAsync(professional.Id);

        // 3. Mapear a DTO
        return jobs.Select(j => new JobDto
        {
            Id = j.Id,
            ClientId = j.ClientId,
            ProfessionalId = j.ProfessionalId ?? Guid.Empty,
            Specialty = j.Specialty,
            Description = j.Description,
            Address = j.Address,
            ScheduledDate = j.ScheduledDate,
            ScheduledHour = j.ScheduledHour,
            TotalCost = j.TotalCost,
            Status = j.Status.ToString()
        });
    }
}