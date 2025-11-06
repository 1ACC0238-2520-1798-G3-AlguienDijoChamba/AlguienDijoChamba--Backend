
using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Commands;

public class AssignTagsToProfessionalCommandHandler(
    IReputationRepository reputationRepository, // Asumo que maneja la persistencia de Tags/ProfessionalTags
    IUnitOfWork unitOfWork) 
    : IRequestHandler<AssignTagsToProfessionalCommand, bool>
{
    public async Task<bool> Handle(AssignTagsToProfessionalCommand request, CancellationToken cancellationToken)
    {
        // 1. Eliminar todas las etiquetas existentes del profesional.
        reputationRepository.RemoveAllTagsByProfessionalId(request.ProfessionalId);
        
        // 2. Crear las nuevas entidades ProfessionalTag para la lista de IDs recibida.
        var newProfessionalTags = request.TagIds
            .Select(tagId => new ProfessionalTag(request.ProfessionalId, tagId))
            .ToList();
            
        // 3. Añadir las nuevas etiquetas a la base de datos.
        reputationRepository.AddRange(newProfessionalTags);
        
        // 4. Guardar los cambios.
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}