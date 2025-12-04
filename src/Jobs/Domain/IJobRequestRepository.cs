namespace AlguienDijoChamba.Api.Jobs.Domain;

public interface IJobRequestRepository 
{ 
    void Add(JobRequest jobRequest);
    
    // ✨ NUEVOS MÉTODOS AGREGADOS para Active Jobs
    Task<JobRequest?> GetByIdAsync(Guid jobRequestId);
    Task<JobRequest?> GetActiveJobByClientAsync(Guid clientId);

    // ✨ NUEVO: listar todos los jobs de un cliente
    Task<IReadOnlyList<JobRequest>> GetByClientAsync(Guid clientId);

    // ✨ EXISTENTE: actualizar un job
    Task UpdateAsync(JobRequest jobRequest);
}
