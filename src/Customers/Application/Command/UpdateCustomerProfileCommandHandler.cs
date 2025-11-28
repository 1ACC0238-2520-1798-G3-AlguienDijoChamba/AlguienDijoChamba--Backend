using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.Customers.Domain.Enum;
using AlguienDijoChamba.Api.Customers.Interfaces.Dtos;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;

namespace AlguienDijoChamba.Api.Customers.Application.Command;

public class UpdateCustomerProfileCommandHandler :
    IRequestHandler<UpdateCustomerProfileCommand, CustomerProfileDto>
{
    private readonly ICustomerRepository customerRepository;
    private readonly IUnitOfWork unitOfWork;

    public UpdateCustomerProfileCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        this.customerRepository = customerRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<CustomerProfileDto> Handle(
        UpdateCustomerProfileCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer is null)
            throw new Exception($"Cliente con Id {request.CustomerId} no encontrado.");

        customer.UpdateProfile(
            request.Nombres,
            request.Apellidos,
            request.Celular,
            request.PhotoUrl,
            (PreferredPaymentMethod)request.PreferredPaymentMethod,
            request.AcceptsBookingUpdates,
            request.AcceptsPromotionsAndOffers,
            request.AcceptsNewsletter
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 🔥 AQUÍ DEVUELVES EL DTO COMPLETO
        return new CustomerProfileDto
        {
            Nombres = customer.Nombres,
            Apellidos = customer.Apellidos,
            Celular = customer.Celular,
            PreferredPaymentMethod = customer.PreferredPaymentMethod.ToString(),
            AcceptsBookingUpdates = customer.AcceptsBookingUpdates,
            AcceptsPromotionsAndOffers = customer.AcceptsPromotionsAndOffers,
            AcceptsNewsletter = customer.AcceptsNewsletter
        };
    }
}