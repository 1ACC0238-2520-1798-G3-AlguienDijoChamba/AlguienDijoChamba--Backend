using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Professionals.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Professionals.Application.Queries;

public class GetMyProfileQueryHandler(IProfessionalRepository professionalRepository) 
    : IRequestHandler<GetMyProfileQuery, ProfileResponse?>
{
    public async Task<ProfileResponse?> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var professional = await professionalRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (professional is null) return null;

        // Simulación de datos que aún no están en la entidad
        var professionalLevel = "Gold Professional";
        var starRating = 4.9;
        var completedJobs = 127;
        var availableBalance = 1250.0m;

        return new ProfileResponse(
            $"{professional.Nombres} {professional.Apellidos}", // Usamos Nombre Completo para userName
            professionalLevel,
            starRating,
            completedJobs,
            availableBalance,
            // --- AÑADIMOS ESTOS DATOS PARA EL MÓVIL ---
            professional.Nombres,
            professional.Apellidos,
            professional.Ocupacion,
            professional.Email,
            professional.Celular,
            professional.FechaNacimiento,
            professional.Genero,
            professional.ProfilePhotoUrl
        );
    }
}