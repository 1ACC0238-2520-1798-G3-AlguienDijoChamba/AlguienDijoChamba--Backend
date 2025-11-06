// En: src/Professionals/Infrastructure/Repositories/ProfessionalRepository.cs
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;
using Microsoft.EntityFrameworkCore;

namespace AlguienDijoChamba.Api.Professionals.Infrastructure.Repositories;

public class ProfessionalRepository(AppDbContext context) : IProfessionalRepository
{
    // Métodos existentes
    public void Add(Professional professional) => context.Professionals.Add(professional);
    public async Task<Professional?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await context.Professionals
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
            
    public async Task<Professional?> GetByIdAsync(Guid professionalId, CancellationToken cancellationToken = default) =>
        await context.Professionals
            .FirstOrDefaultAsync(p => p.Id == professionalId, cancellationToken); // <--- Busca por la columna 'Id'
    // --- IMPLEMENTACIÓN REQUERIDA ---
    public void Remove(Professional professional) => context.Professionals.Remove(professional);
}