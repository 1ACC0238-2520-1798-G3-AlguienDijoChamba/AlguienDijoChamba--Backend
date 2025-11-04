using System.Text.Json; 
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;

namespace AlguienDijoChamba.Api.Professionals.Application.Commands;

public class CompleteProfileCommandHandler(IProfessionalRepository professionalRepository, IUnitOfWork unitOfWork)
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
        return true;
    }
}