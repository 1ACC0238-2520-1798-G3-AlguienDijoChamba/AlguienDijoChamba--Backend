namespace AlguienDijoChamba.Api.Reputation.Domain;

public interface IReputationRepository
{
    Task<UserReputationTechnician?> GetByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserReputationTechnician>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(UserReputationTechnician reputation);
    void Update(UserReputationTechnician reputation);
    Task<IEnumerable<Tag>> GetAllTagsAsync(CancellationToken cancellationToken);
    void AddRange(IEnumerable<ProfessionalTag> professionalTags);
    void RemoveAllTagsByProfessionalId(Guid professionalId);
    Task<Tag?> GetTagByIdAsync(Guid tagId, CancellationToken cancellationToken);
    Task<IEnumerable<Tag>> GetTagsByProfessionalIdAsync(Guid professionalId, CancellationToken cancellationToken);
    
    Task<Tag?> GetTagByNameAsync(string name, CancellationToken cancellationToken = default);
    void AddTag(Tag tag);
    Task<IEnumerable<Guid>> GetProfessionalsByTagIdAsync(Guid tagId, CancellationToken cancellationToken);
}