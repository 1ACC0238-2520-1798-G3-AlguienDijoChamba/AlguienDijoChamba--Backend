using AlguienDijoChamba.Api.Jobs.Domain;
using AlguienDijoChamba.Api.Reputation.Application.Commands;
using AlguienDijoChamba.Api.Reputation.Domain; // <-- Importa la Interfaz
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
// Elimina la importación de la clase concreta de Repositories
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application;

// Handler que crea/actualiza la reputación a partir de un Job
public class CreateReputationFromJobCommandHandler
    : IRequestHandler<CreateReputationFromJobCommand, object>
{
    // ✅ Usar la interfaz para que funcione con DI
    private readonly IReputationRepository _reputationRepository;

    public CreateReputationFromJobCommandHandler(IReputationRepository reputationRepository)
    {
        _reputationRepository = reputationRepository;
    }

    public async Task<object> Handle(CreateReputationFromJobCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"🔵 Handler: Procesando CreateReputationFromJobCommand - JobId: {request.JobId}");
        
        // ⚠️ Implementación provisional: devuelve un DTO “dummy”
        return new ReputationDto
        {
            Id = Guid.NewGuid(),
            ProfessionalId = Guid.NewGuid(),
            StarRating = request.Rating,
            CompletedJobs = 1,
            ProfessionalLevel = "Bronze Professional",
            HourlyRate = 0m
        };
    }
}
