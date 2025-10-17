using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;
using Microsoft.EntityFrameworkCore;

namespace AlguienDijoChamba.Api.Professionals.Infrastructure.Repositories;
public class ProfessionalRepository(AppDbContext context) : IProfessionalRepository
{
    public void Add(Professional professional)
    {
        context.Professionals.Add(professional);
    }
    public async Task<Professional?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Professionals
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }
    
}