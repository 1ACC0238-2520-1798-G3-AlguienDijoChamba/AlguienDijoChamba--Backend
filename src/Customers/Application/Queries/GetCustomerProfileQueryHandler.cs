using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.Customers.Domain.Enum;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

namespace AlguienDijoChamba.Api.Customers.Application.Queries;

// El Handler que MediatR busca
public class CustomerProfileQueryHandler(ICustomerRepository repository) 
    : IRequestHandler<GetCustomerProfileQuery, CustomerProfileDto>
{
    public async Task<CustomerProfileDto> Handle(GetCustomerProfileQuery query, CancellationToken cancellationToken)
    {
        // 🚀 CORRECCIÓN CLAVE: Usamos GetByUserIdAsync para buscar por el ID de usuario (Customer.UserId)
        // **NOTA:** Asumimos que ICustomerRepository ahora tiene este método.
        var customer = await repository.GetByUserIdAsync(query.UserId, cancellationToken); 

        if (customer == null)
        {
            // Lanza excepción si no se encuentra (para que el Controller devuelva 404)
            throw new Exception($"Perfil de cliente con UserID {query.UserId} no encontrado."); 
        }

        // Mapear la entidad de dominio al DTO de respuesta
        return new CustomerProfileDto
        {
            Nombres = customer.Nombres,
            Apellidos = customer.Apellidos,
            Celular = customer.Celular,
            // Mapeo (ej: enum a string)
            PreferredPaymentMethod = customer.PreferredPaymentMethod.ToString(), 
            AcceptsBookingUpdates = customer.AcceptsBookingUpdates,
            AcceptsPromotionsAndOffers = customer.AcceptsPromotionsAndOffers,
            AcceptsNewsletter = customer.AcceptsNewsletter
        };
    }
}