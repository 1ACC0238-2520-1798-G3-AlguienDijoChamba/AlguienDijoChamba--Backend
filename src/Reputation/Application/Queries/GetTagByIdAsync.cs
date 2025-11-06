// En: AlguienDijoChamba.Api.Reputation.Application.Queries

using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

public class GetTagByIdQueryHandler(IReputationRepository repository) 
    : IRequestHandler<GetTagByIdQuery, TagDto?>
{
    public async Task<TagDto?> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await repository.GetTagByIdAsync(request.TagId, cancellationToken);
        
        if (tag is null) return null;
        
        // Mapea la entidad Tag a TagDto para la respuesta
        return new TagDto(tag.Id, tag.Name);
    }
}