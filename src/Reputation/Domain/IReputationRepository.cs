namespace AlguienDijoChamba.Api.Reputation.Domain;

public interface IReputationRepository
{
    Task<UserReputationTechnician?> GetByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserReputationTechnician>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(UserReputationTechnician reputation);
    void Update(UserReputationTechnician reputation);
}