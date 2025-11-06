using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Commands;

public class CreateInitialReputationCommandHandler(
    IReputationRepository reputationRepository, 
    IUnitOfWork unitOfWork) 
    : IRequestHandler<CreateInitialReputationCommand, UserReputationTechnician>
{
    public async Task<UserReputationTechnician> Handle(CreateInitialReputationCommand request, CancellationToken cancellationToken)
    {
        // 1. Verificar si la reputación ya existe
        var existingReputation = await reputationRepository.GetByProfessionalIdAsync(request.ProfessionalId, cancellationToken);
        
        if (existingReputation is not null)
        {
            // Si ya existe, devolvemos el registro existente.
            return existingReputation;
        }

        // 2. Crear la entidad inicial (con los nombres corregidos)
        var newReputation = new UserReputationTechnician(
            professionalId: request.ProfessionalId,
            // ⬅️ CORRECCIÓN DE NOMENCLATURA
            starRating: 5.0, 
            // ⬅️ CORRECCIÓN DE NOMENCLATURA
            completedJobs: 0,
            // ⬅️ CORRECCIÓN DE NOMENCLATURA
            professionalLevel: "Bronze Professional", 
            hourlyRate: request.InitialHourlyRate 
        );
        
        // 3. Persistir el nuevo registro
        reputationRepository.Add(newReputation);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return newReputation;
    }
}