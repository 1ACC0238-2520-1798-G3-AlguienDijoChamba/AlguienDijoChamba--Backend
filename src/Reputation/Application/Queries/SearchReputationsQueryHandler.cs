using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Reputation.Domain;
using MediatR;
// Asegúrate de que este using apunta a tu Repositorio

// Necesario para la lógica de filtros

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

public class SearchReputationsQueryHandler : IRequestHandler<SearchReputationsQuery, object> 
{
    private readonly IReputationRepository _reputationRepository; 
    private readonly IProfessionalRepository _professionalRepository; 
    
    // Constructor con inyección
    public SearchReputationsQueryHandler(IReputationRepository reputationRepository, IProfessionalRepository professionalRepository) 
    { 
        _reputationRepository = reputationRepository;
        _professionalRepository = professionalRepository; 
    }
    
    public async Task<object> Handle(SearchReputationsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Guid>? finalProfessionalIds = null;

        // 1. FILTRO POR NOMBRE (Busca IDs en el repositorio de Profesionales)
        if (!string.IsNullOrWhiteSpace(request.searchTerm))
        {
            // 🚀 CORRECCIÓN: Usamos 'await' para la llamada asíncrona.
            var idsMatchingName = await _professionalRepository.FindProfessionalIdsByTermAsync(request.searchTerm, cancellationToken);

            if (idsMatchingName == null || !idsMatchingName.Any())
            {
                // Si el nombre no existe, devolvemos un resultado vacío.
                return new List<object>(); 
            }
            finalProfessionalIds = idsMatchingName;
        }

        // 2. FILTRO POR TAGS (Viene como una cadena de IDs, la parseamos y filtramos)
        if (!string.IsNullOrWhiteSpace(request.professionalIds))
        {
            // Parseamos la cadena de IDs separada por comas en una lista de Guid
            var idsFromTags = request.professionalIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Where(s => Guid.TryParse(s.Trim(), out _))
                .Select(s => Guid.Parse(s.Trim()))
                .ToList();
            
            if (finalProfessionalIds == null)
            {
                // Solo había filtro de tags
                finalProfessionalIds = idsFromTags;
            }
            else
            {
                // Había filtro de nombre Y filtro de tags => INTERSECCIÓN (AND)
                finalProfessionalIds = finalProfessionalIds.Intersect(idsFromTags);
            }
        }
        
        // 3. Si no hay ningún filtro (ni nombre, ni tags), finalProfessionalIds será null.
        // El ReputationRepository debe manejar la devolución de TODOS los resultados en ese caso.

        // 4. Ejecutar la Búsqueda Final en el Repositorio de Reputación
        var result = await _reputationRepository.SearchReputationsAsync(
            professionalIds: finalProfessionalIds,
            page: request.page,
            limit: request.limit,
            cancellationToken: cancellationToken
        );

        return result;
    }
}