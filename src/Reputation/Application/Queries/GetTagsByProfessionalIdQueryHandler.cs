
using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

public class GetTagsByProfessionalIdQueryHandler(IReputationRepository repository) 
    : IRequestHandler<GetTagsByProfessionalIdQuery, IEnumerable<TagDto>>
{
    public async Task<IEnumerable<TagDto>> Handle(GetTagsByProfessionalIdQuery request, CancellationToken cancellationToken)
    {
        var tags = await repository.GetTagsByProfessionalIdAsync(request.ProfessionalId, cancellationToken);
        
        // Mapea la lista de entidades Tag a List<TagDto>
        return tags.Select(t => new TagDto(t.Id, t.Name)).ToList();
    }
}