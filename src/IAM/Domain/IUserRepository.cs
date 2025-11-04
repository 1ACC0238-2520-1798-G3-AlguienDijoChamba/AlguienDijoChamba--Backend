// En: src/IAM/Domain/IUserRepository.cs
using AlguienDijoChamba.Api.IAM.Domain;

namespace AlguienDijoChamba.Api.IAM.Domain;

public interface IUserRepository
{
    // Método existente
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Add(User user);
    
    // --- MÉTODOS REQUERIDOS PARA ELIMINACIÓN ---
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Remove(User user);
}