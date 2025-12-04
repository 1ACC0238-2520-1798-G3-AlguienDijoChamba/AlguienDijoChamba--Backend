using MediatR;
using AlguienDijoChamba.Api.Customers.Interfaces.Dtos; // ⚠️ Asegúrate de incluir el using para el DTO

namespace AlguienDijoChamba.Api.Customers.Application.Command;

// Comando para completar la información del registro (Etapa 2)
public record CompleteCustomerProfileCommand(
    Guid UserId,
    int PreferredPaymentMethod, 
    bool AcceptsBookingUpdates,
    bool AcceptsPromotionsAndOffers,
    bool AcceptsNewsletter
) : IRequest<CustomerCompleteRegistrationRequest>; 