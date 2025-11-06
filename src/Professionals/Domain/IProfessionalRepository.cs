// En: src/Professionals/Domain/IProfessionalRepository.cs
using AlguienDijoChamba.Api.Professionals.Domain;

namespace AlguienDijoChamba.Api.Professionals.Domain;

public interface IProfessionalRepository
{
    void Add(Professional professional);
    Task<Professional?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<Professional?> GetByIdAsync(Guid professionalId, CancellationToken cancellationToken); 
    
    // --- MÉTODO REQUERIDO PARA ELIMINACIÓN ---
    void Remove(Professional professional);
}