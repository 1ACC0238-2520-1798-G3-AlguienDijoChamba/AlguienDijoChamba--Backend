using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.Customers.Domain.Enum;
// 🚀 USING QUE FALTABA: NECESARIO PARA CONOCER EL DTO
using AlguienDijoChamba.Api.Customers.Interfaces.Dtos; 
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

// ⚠️ CAMBIO 1: El Command Handler debe devolver el tipo de Request para confirmar los datos
namespace AlguienDijoChamba.Api.Customers.Application.Command;

public class CompleteCustomerProfileCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CompleteCustomerProfileCommand, CustomerCompleteRegistrationRequest>
{
    public async Task<CustomerCompleteRegistrationRequest> Handle(CompleteCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (customer is null)
            throw new Exception($"Cliente con Id {request.UserId} no encontrado para completar registro."); 

        customer.UpdatePreferences( 
            (PreferredPaymentMethod)request.PreferredPaymentMethod, 
            request.AcceptsBookingUpdates,
            request.AcceptsPromotionsAndOffers,
            request.AcceptsNewsletter
        );
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // 🚀 CAMBIO 2: Devolvemos los datos del Request para confirmar que se procesaron
        return new CustomerCompleteRegistrationRequest
        {
            PreferredPaymentMethod = request.PreferredPaymentMethod,
            AcceptsBookingUpdates = request.AcceptsBookingUpdates,
            AcceptsPromotionsAndOffers = request.AcceptsPromotionsAndOffers,
            AcceptsNewsletter = request.AcceptsNewsletter
        };
    }
}