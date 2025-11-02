using AlguienDijoChamba.Api.IAM.Domain;

namespace AlguienDijoChamba.Api.Customers.Domain;

public interface ICustomerRepository
{
    void Add(Customer customer);
    Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}