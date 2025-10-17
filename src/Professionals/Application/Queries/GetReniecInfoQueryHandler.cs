using AlguienDijoChamba.Api.Professionals.Domain;
using MediatR;

namespace AlguienDijoChamba.Api.Professionals.Application.Queries;

public class GetReniecInfoQueryHandler(IReniecService reniecService)
    : IRequestHandler<GetReniecInfoQuery, ReniecInfo?> // Implementa la interfaz de MediatR
{
    public async Task<ReniecInfo?> Handle(GetReniecInfoQuery request, CancellationToken cancellationToken)
    {
        // Simplemente delega la llamada al servicio de RENIEC
        return await reniecService.GetReniecInfoByDni(request.Dni, cancellationToken);
    }
}