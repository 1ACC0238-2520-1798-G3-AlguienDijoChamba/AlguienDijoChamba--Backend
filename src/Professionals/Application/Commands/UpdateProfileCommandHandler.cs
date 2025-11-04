using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;

namespace AlguienDijoChamba.Api.Professionals.Application.Commands;

public class UpdateProfileCommandHandler(IProfessionalRepository professionalRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProfileCommand, bool>
{
    public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var professional = await professionalRepository.GetByUserIdAsync(request.UserId, cancellationToken)
                           ?? throw new Exception("Perfil de profesional no encontrado.");

        professional.UpdateDetails(
            request.Email,
            request.Celular,
            request.Ocupacion,
            request.FechaNacimiento,
            request.Genero
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}