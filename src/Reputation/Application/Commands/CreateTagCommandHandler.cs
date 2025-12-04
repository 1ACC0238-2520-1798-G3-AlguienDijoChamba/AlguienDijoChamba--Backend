using MediatR;
using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using AlguienDijoChamba.Api.Shared.Domain.Repositories; 

namespace AlguienDijoChamba.Api.Reputation.Application.Commands
{
    public class CreateTagCommandHandler(
        IReputationRepository reputationRepository, 
        IUnitOfWork unitOfWork) 
        : IRequestHandler<CreateTagCommand, TagDto>
    {
        public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            // Normalizar el nombre a mayúsculas para asegurar unicidad
            var tagName = request.Name.ToUpper();
            
            // 1. Verificar si ya existe el tag
            var existingTag = await reputationRepository.GetTagByNameAsync(tagName, cancellationToken);
            
            if (existingTag is not null)
            {
                // Devolver el existente para evitar duplicados en la base de datos
                return new TagDto(existingTag.Id, existingTag.Name); 
            }

            // 2. Crear la entidad de dominio
            var newTag = new Tag(tagName); 
            
            // 3. Agregar al contexto de la base de datos
            reputationRepository.AddTag(newTag); 

            // 4. Guardar los cambios (COMMIT)
            await unitOfWork.SaveChangesAsync(cancellationToken); 

            // 5. Mapear la entidad al DTO de respuesta
            return new TagDto(newTag.Id, newTag.Name);
        }
    }
}