using MediatR;
using System.Collections.Generic;
using System.Linq; // Necesario para .ToList()
using System.Threading;
using System.Threading.Tasks;
using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

// No se inyecta IMapper, solo el repositorio.
public class GetAllReputationsQueryHandler(IReputationRepository reputationRepository) 
    : IRequestHandler<GetAllReputationsQuery, IEnumerable<ReputationDto>>
{
    public async Task<IEnumerable<ReputationDto>> Handle(
        GetAllReputationsQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Obtener todos los modelos de Reputación de la base de datos.
        // Asumo que tu repositorio devuelve una lista de entidades Reputation.
        var reputations = await reputationRepository.GetAllAsync();
        
        // 2. Mapear manualmente (sin AutoMapper) la entidad de dominio al DTO.
        var reputationDtos = reputations.Select(r => new ReputationDto
        {
            Id = r.Id,
            ProfessionalId = r.ProfessionalId,
            Rating = r.Rating,
            ReviewsCount = r.ReviewsCount,
            Level = r.Level,
            HourlyRate = r.HourlyRate
        }).ToList();
        
        return reputationDtos;
    }
}