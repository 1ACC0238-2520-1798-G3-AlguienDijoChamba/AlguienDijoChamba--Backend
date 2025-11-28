using AlguienDijoChamba.Api.Customers.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Customers.Application.Queries;

// Comando para solicitar los datos del cliente
public class GetCustomerProfileQuery : IRequest<CustomerProfileDto>
{
    public Guid UserId { get; set; }
}