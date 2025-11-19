using AlguienDijoChamba.Api.Customers.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Customers.Application.Command;

public record UpdateCustomerProfileCommand(
    Guid CustomerId,
    string Nombres,
    string Apellidos,
    string Celular,
    string PhotoUrl,
    int PreferredPaymentMethod,
    bool AcceptsBookingUpdates,
    bool AcceptsPromotionsAndOffers,
    bool AcceptsNewsletter
) : IRequest<CustomerProfileDto>;