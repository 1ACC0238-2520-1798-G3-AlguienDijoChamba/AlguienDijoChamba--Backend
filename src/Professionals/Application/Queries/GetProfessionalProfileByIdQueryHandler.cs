using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Professionals.Interfaces.Dtos;
using AlguienDijoChamba.Api.Reputation.Application.Queries;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Professionals.Application.Queries;

public class GetProfessionalProfileByIdQueryHandler(
    IProfessionalRepository professionalRepository,
    ISender sender) 
    : IRequestHandler<GetProfessionalProfileByIdQuery, ProfileResponse?>
{
    public async Task<ProfileResponse?> Handle(GetProfessionalProfileByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Obtener Entidad Professional
        var professional = await professionalRepository.GetByIdAsync(request.ProfessionalId, cancellationToken);        if (professional is null) return null;

        // 2. Obtener Reputación
        var reputationQuery = new GetReputationByProfessionalIdQuery(request.ProfessionalId);
        ReputationResponse? reputation = await sender.Send(reputationQuery, cancellationToken);

        // 3. Preparar los 13 valores

        // Valores de Reputación
        var starRating = reputation?.StarRating ?? 0.0;
        var completedJobs = reputation?.CompletedJobs ?? 0;
        var professionalLevel = reputation?.ProfessionalLevel ?? "Bronze Professional";
        
        // Valores que asumo existen en la Entidad/Módulo Professional
        var availableBalance = 0.0m; // ⚠️ Simulado, ajusta si es necesario
        var hourlyRate = professional.HourlyRate; // Precio

        // 4. 🚀 Invocar el constructor de ProfileResponse con 13 argumentos posicionales
        return new ProfileResponse(
            $"{professional.Nombres} {professional.Apellidos}", 
            
            professionalLevel, 
            
            starRating,
            
            completedJobs,
            
            availableBalance,
            
            // ARG 7: Nombres
            professional.Nombres,
            
            // ARG 8: Apellidos
            professional.Apellidos,
            
            // ARG 9: Ocupacion
            professional.Ocupacion,
            
            // ARG 10: Email
            professional.Email,
            
            // ARG 11: Celular
            professional.Celular,
            
            // ARG 12: FechaNacimiento
            professional.FechaNacimiento,
            
            // ARG 13: Genero
            professional.Genero,
            
            professional.ProfilePhotoUrl
            
        );
    }
}