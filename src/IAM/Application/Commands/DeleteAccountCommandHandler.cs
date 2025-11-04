// En: src/IAM/Application/Commands/DeleteAccountCommandHandler.cs
using AlguienDijoChamba.Api.Professionals.Domain; // Para el IProfessionalRepository
using AlguienDijoChamba.Api.IAM.Domain; // Para el IUserRepository
using AlguienDijoChamba.Api.Shared.Domain.Repositories; // Para el IUnitOfWork
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AlguienDijoChamba.Api.IAM.Application.Commands;

public class DeleteAccountCommandHandler(
    IUserRepository userRepository,
    IProfessionalRepository professionalRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener el perfil y el usuario
        var professional = await professionalRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken); // *Requiere GetByIdAsync en IUserRepository*
        
        // 2. Lógica de Eliminación (asume que los repositorios tienen el método Remove)
        if (professional is not null) professionalRepository.Remove(professional); 
        if (user is not null) userRepository.Remove(user); 

        // 3. Guardar cambios
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}