using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Commands;

public class RecalculateReputationCommandHandler(
    IReputationRepository reputationRepository, 
    IUnitOfWork unitOfWork) 
    : IRequestHandler<RecalculateReputationCommand, UserReputationTechnician>
{
    public async Task<UserReputationTechnician> Handle(RecalculateReputationCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la entidad existente
        var reputationEntity = await reputationRepository.GetByProfessionalIdAsync(request.ProfessionalId, cancellationToken);
        
        if (reputationEntity is null)
        {
            // ⬅️ CREACIÓN INICIAL (FALLBACK): Usando los nombres corregidos
            reputationEntity = new UserReputationTechnician(
                professionalId: request.ProfessionalId,
                starRating: request.NewRatingValue,  // Antes: rating
                completedJobs: 1,                    // Antes: reviewsCount
                professionalLevel: "Bronze Professional", // Antes: level
                hourlyRate: 0.0m 
            );
            reputationRepository.Add(reputationEntity);
        }
        else
        {
            // 2. CALCULAR EL NUEVO PROMEDIO PONDERADO
            
            // ⬅️ Usando los nombres corregidos de la entidad
            double currentRating = reputationEntity.StarRating;
            int currentReviewsCount = reputationEntity.CompletedJobs;
            
            // a) Calcular el total acumulado de puntos y el nuevo contador
            double currentTotalPoints = currentRating * currentReviewsCount;
            int newReviewsCount = currentReviewsCount + 1;
            double newTotalPoints = currentTotalPoints + request.NewRatingValue;
            
            double newAverageRating = newTotalPoints / newReviewsCount;

            // 3. Lógica de Nivel y Actualización
            
            // Determinar el nuevo nivel (usando el nombre corregido)
            string newLevel = reputationEntity.ProfessionalLevel;
            if (newReviewsCount > 50 && newAverageRating >= 4.8)
            {
                newLevel = "Gold Professional";
            }
            else if (newReviewsCount > 10)
            {
                newLevel = "Silver Professional";
            }
            // Si el nivel no cambia, se mantiene el valor actual.
            
            // ⬅️ Usando el método UpdateDetails corregido
            reputationEntity.UpdateDetails(newAverageRating, newReviewsCount, newLevel, reputationEntity.HourlyRate);
        }

        // 4. PERSISTIR LOS CAMBIOS
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return reputationEntity;
    }
}