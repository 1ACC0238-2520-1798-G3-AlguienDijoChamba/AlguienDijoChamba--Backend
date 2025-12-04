// Nuevo Handler, usando tu repositorio

using AlguienDijoChamba.Api.Reputation.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

public class GetProfessionalsByTagHandler(IReputationRepository repository) 
    : IRequestHandler<GetProfessionalsByTagQuery, IEnumerable<Guid>>
{
    public async Task<IEnumerable<Guid>> Handle(GetProfessionalsByTagQuery request, CancellationToken cancellationToken)
    {
        // El repositorio se encarga de ir a ProfessionalTag en la BD
        return await repository.GetProfessionalsByTagIdAsync(request.TagId, cancellationToken);
    }
}