namespace AlguienDijoChamba.Api.Professionals.Domain;

public interface IProfessionalRepository
{
    void Add(Professional professional);
    Task<Professional?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}