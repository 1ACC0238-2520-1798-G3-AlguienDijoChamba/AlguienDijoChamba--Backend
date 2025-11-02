using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.IAM.Domain;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;
using Microsoft.EntityFrameworkCore;

namespace AlguienDijoChamba.Api.Customers.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Customer customer) => _context.Customers.Add(customer);

    public async Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
}