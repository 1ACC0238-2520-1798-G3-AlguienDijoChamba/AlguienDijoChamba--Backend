using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.IAM.Application.Commands;
using AlguienDijoChamba.Api.IAM.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AlguienDijoChamba.Api.Customers.Application.Command;

public class RegisterCustomerCommandHandler(
    IUserRepository userRepository,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher<User> passwordHasher
) : IRequestHandler<RegisterCustomerCommand, Guid>
{
    public async Task<Guid> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.GetByEmailAsync(request.Email, cancellationToken) is not null)
            throw new Exception("El correo electrónico ya está en uso.");

        var passwordHash = passwordHasher.HashPassword(null!, request.Password);
        var user = User.Create(request.Email, passwordHash);
        userRepository.Add(user);

        var customer = Customer.Create(user.Id, request.Nombres, request.Apellidos, request.Celular);
        customerRepository.Add(customer);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}