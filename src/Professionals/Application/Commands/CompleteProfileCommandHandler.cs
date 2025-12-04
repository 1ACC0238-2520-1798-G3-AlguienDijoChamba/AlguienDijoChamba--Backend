using System.Text.Json; 
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using AlguienDijoChamba.Api.Reputation.Application.Commands;
namespace AlguienDijoChamba.Api.Professionals.Application.Commands;

public class CompleteProfileCommandHandler(IProfessionalRepository professionalRepository, IUnitOfWork unitOfWork,ISender sender)
    : IRequestHandler<CompleteProfileCommand, bool>
{
    public async Task<bool> Handle(CompleteProfileCommand request, CancellationToken cancellationToken)
    {
        var professional = await professionalRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (professional is null)
        {
            throw new Exception("Perfil de profesional no encontrado.");
        }

        string? certUrlsJson = null;
        if (request.CertificationUrls != null && request.CertificationUrls.Count > 0)
        {
            certUrlsJson = JsonSerializer.Serialize(request.CertificationUrls);
        }

        // 2. CORRECCIÓN: Llama a UpdateProfile con los 5 argumentos
        professional.UpdateProfile(
            request.YearsOfExperience, 
            request.HourlyRate, 
            request.ProfessionalBio,
            request.ProfilePhotoUrl,     
            certUrlsJson                 
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);
        // ---------------------------------------------------------
        // 🚀 CORRECCIÓN CLAVE: CREAR LA REPUTACIÓN AUTOMÁTICAMENTE
        // ---------------------------------------------------------
        // Si el técnico puso una tarifa, la usamos. Si no, 0.
        decimal initialRate = request.HourlyRate ?? 0m;

        var createReputationCommand = new CreateInitialReputationCommand(
            professional.Id, // Usamos el ID del Professional (no el UserId)
            initialRate
        );

        // Esto insertará el registro en la tabla Reputations
        await sender.Send(createReputationCommand, cancellationToken);
        return true;
    }
}