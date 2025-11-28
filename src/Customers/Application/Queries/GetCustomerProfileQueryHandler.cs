using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.Customers.Domain.Enum;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

namespace AlguienDijoChamba.Api.Customers.Application.Queries;

public class CustomerProfileQueryHandler(ICustomerRepository repository) 
    : IRequestHandler<GetCustomerProfileQuery, CustomerProfileDto>
{
    public async Task<CustomerProfileDto> Handle(GetCustomerProfileQuery query, CancellationToken cancellationToken)
    {
        Console.WriteLine($"🔍 GET PROFILE QUERY: Buscando por UserId: {query.UserId}");
        
        // ✅ PRIMERO: Intentar por UserId (ID real del usuario, f7e2...)
        var customer = await repository.GetByUserIdAsync(query.UserId, cancellationToken);
        
        Console.WriteLine($"🔍 GetByUserIdAsync resultado: {(customer != null ? "Encontrado" : "No encontrado")}");
        
        // ✅ SI NO ENCUENTRA: Intentar por Id (customerId, 3fb3...)
        if (customer == null)
        {
            Console.WriteLine($"⚠️ No encontrado por UserId, intentando por Id (customerId): {query.UserId}");
            customer = await repository.GetByIdAsync(query.UserId, cancellationToken);
            Console.WriteLine($"🔍 GetByIdAsync resultado: {(customer != null ? "Encontrado" : "No encontrado")}");
        }
        
        if (customer == null)
        {
            throw new Exception($"Perfil de cliente no encontrado para ID: {query.UserId}"); 
        }

        // Mapear la entidad de dominio al DTO de respuesta
        return new CustomerProfileDto
        {
            Id = customer.Id,
            UserId = customer.UserId,
            Nombres = customer.Nombres,
            Apellidos = customer.Apellidos,
            Celular = customer.Celular,
            PhotoUrl = customer.PhotoUrl,
            PreferredPaymentMethod = customer.PreferredPaymentMethod.ToString(), 
            AcceptsBookingUpdates = customer.AcceptsBookingUpdates,
            AcceptsPromotionsAndOffers = customer.AcceptsPromotionsAndOffers,
            AcceptsNewsletter = customer.AcceptsNewsletter
        };
    }
}
