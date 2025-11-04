// En: AlguienDijoChamba.Api.Professionals.Application.Commands/CompleteProfileCommandHandler.cs
using System.Text.Json; // <-- Importa el serializador JSON
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

        // --- LÓGICA ACTUALIZADA ---
        
        // 1. Serializa la lista de URLs de certificaciones a un string JSON
        string? certUrlsJson = null;
        if (request.CertificationUrls != null && request.CertificationUrls.Count > 0)
        {
            certUrlsJson = JsonSerializer.Serialize(request.CertificationUrls);
        }

        // 2. Llama al método de dominio actualizado
        professional.UpdateProfile(
            request.YearsOfExperience, 
            request.HourlyRate, 
            request.ProfessionalBio,
            request.ProfilePhotoUrl, // Pasa la URL de la foto
            certUrlsJson             // Pasa el JSON de las certificaciones
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}