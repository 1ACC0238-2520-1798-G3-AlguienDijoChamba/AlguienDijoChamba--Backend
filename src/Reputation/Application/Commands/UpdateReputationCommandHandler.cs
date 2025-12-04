using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories; // Necesario para IUnitOfWork
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AlguienDijoChamba.Api.Reputation.Application.Commands;

public class UpdateReputationCommandHandler(
    IReputationRepository repository,
    IUnitOfWork unitOfWork) // ⬅️ Inyección de IUnitOfWork
    : IRequestHandler<UpdateReputationCommand, UserReputationTechnician>
{
    public async Task<UserReputationTechnician> Handle(UpdateReputationCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByProfessionalIdAsync(request.ProfessionalId, cancellationToken);
        UserReputationTechnician result;

        if (existing is null)
        {
            // --- 1. CREACIÓN (POST) ---
            result = new UserReputationTechnician(
                request.ProfessionalId, 
                request.Rating, 
                request.ReviewsCount, 
                request.Level, 
                request.HourlyRate
            );
            repository.Add(result);
        }
        else
        {
            // --- 2. ACTUALIZACIÓN (PUT) ---
            // Usamos un nuevo método para actualizar todas las propiedades de una vez
            // (esto requiere un pequeño cambio en la entidad, ver abajo).
            existing.UpdateAll(
                request.Rating,
                request.ReviewsCount,
                request.Level,
                request.HourlyRate
            );
            result = existing;
            repository.Update(result);
        }
        
        // 3. Persistir los cambios hechos por el Handler
        await unitOfWork.SaveChangesAsync(cancellationToken); 

        return result;
    }
}