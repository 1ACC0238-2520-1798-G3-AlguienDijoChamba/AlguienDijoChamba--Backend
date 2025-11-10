using AlguienDijoChamba.Api.Jobs.Domain;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;
using Microsoft.EntityFrameworkCore;

namespace AlguienDijoChamba.Api.Jobs.Infrastructure.Repositories;

public class JobRequestRepository : IJobRequestRepository
{
    private readonly AppDbContext _context;

    public JobRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(JobRequest jobRequest)
    {
        _context.JobRequests.Add(jobRequest);
    }

    public async Task<JobRequest?> GetByIdAsync(Guid jobRequestId)
    {
        return await _context.JobRequests
            .FirstOrDefaultAsync(j => j.Id == jobRequestId);
    }

    public async Task<JobRequest?> GetActiveJobByClientAsync(Guid clientId)
    {
        return await _context.JobRequests
            .Where(j => j.ClientId == clientId && j.Status == JobRequestStatus.Accepted)
            .OrderByDescending(j => j.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(JobRequest jobRequest)
    {
        _context.JobRequests.Update(jobRequest);
        await _context.SaveChangesAsync();
    }
}
