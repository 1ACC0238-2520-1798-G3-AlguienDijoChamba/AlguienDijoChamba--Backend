using MediatR;
using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

public class GetAllReputationsQueryHandler(IReputationRepository reputationRepository) 
    : IRequestHandler<GetAllReputationsQuery, IEnumerable<ReputationDto>>
{
    public async Task<IEnumerable<ReputationDto>> Handle(
        GetAllReputationsQuery request, 
        CancellationToken cancellationToken)
    {
        var reputations = await reputationRepository.GetAllAsync();
        
        // 2. Mapear manualmente la entidad de dominio al DTO.
        var reputationDtos = reputations.Select(r => new ReputationDto
        {
            Id = r.Id,
            ProfessionalId = r.ProfessionalId,
            
            // ⬅️ CORRECCIÓN: Usando los nuevos nombres de la entidad UserReputationTechnician
            StarRating = r.StarRating,         
            CompletedJobs = r.CompletedJobs,  
            ProfessionalLevel = r.ProfessionalLevel, 
            
            HourlyRate = r.HourlyRate
            
        }).ToList();
        
        return reputationDtos;
    }
}