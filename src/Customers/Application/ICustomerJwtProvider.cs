using AlguienDijoChamba.Api.IAM.Domain;

namespace AlguienDijoChamba.Api.Customers.Application;

public interface ICustomerJwtProvider
{
    string Generate(User user, string role); 
}