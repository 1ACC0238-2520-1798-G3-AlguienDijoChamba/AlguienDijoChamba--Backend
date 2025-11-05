using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;
using Microsoft.EntityFrameworkCore;

namespace AlguienDijoChamba.Api.Reputation.Infrastructure.Repositories;

public class ReputationRepository(AppDbContext context) : IReputationRepository
{
    public async Task<UserReputationTechnician?> GetByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken = default)
        => await context.Set<UserReputationTechnician>()
            .FirstOrDefaultAsync(r => r.ProfessionalId == professionalId, cancellationToken);

    public async Task<IEnumerable<UserReputationTechnician>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Set<UserReputationTechnician>().ToListAsync(cancellationToken);

    // Simple como UserRepository
    public void Add(UserReputationTechnician reputation) => context.Set<UserReputationTechnician>().Add(reputation);

    public void Update(UserReputationTechnician reputation) => context.Set<UserReputationTechnician>().Update(reputation);
}