using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

public class GetAllTagsQueryHandler(IReputationRepository repository) 
    : IRequestHandler<GetAllTagsQuery, IEnumerable<TagDto>> // ⬅️ Devuelve TagDto
{
    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await repository.GetAllTagsAsync(cancellationToken);
        
        // 🚀 Mapeo al DTO (usando Select o un Mapper como AutoMapper si lo tienes)
        return tags.Select(t => new TagDto(t.Id, t.Name)).ToList();
    }
}