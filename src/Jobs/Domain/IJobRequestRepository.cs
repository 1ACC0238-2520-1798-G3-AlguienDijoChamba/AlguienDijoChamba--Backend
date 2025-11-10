namespace AlguienDijoChamba.Api.Jobs.Domain;

public interface IJobRequestRepository 
{ 
    void Add(JobRequest jobRequest);
    
    // ✨ NUEVOS MÉTODOS AGREGADOS para Active Jobs
    Task<JobRequest?> GetByIdAsync(Guid jobRequestId);
    Task<JobRequest?> GetActiveJobByClientAsync(Guid clientId);
    Task UpdateAsync(JobRequest jobRequest);
}
