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
    
    public async Task<IEnumerable<Guid>> FindProfessionalIdsByTermAsync(string term, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            // Si el término es nulo o vacío, devolvemos una lista vacía para no filtrar nada
            return Enumerable.Empty<Guid>();
        }

        var lowerTerm = term.Trim().ToLower();

        // Filtra la tabla Professional por Nombres o Apellidos y proyecta solo el Id
        var professionalIds = await context.Set<Professional>() // Asumo que el DbSet es <Professional>
            .Where(p => 
                // Búsqueda insensible a mayúsculas/minúsculas
                p.Nombres.ToLower().Contains(lowerTerm) || 
                p.Apellidos.ToLower().Contains(lowerTerm)
            )
            // Selecciona solo el ID
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        return professionalIds;
    }
}