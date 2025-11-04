using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.IAM.Application.Queries;
using AlguienDijoChamba.Api.IAM.Domain;
using AlguienDijoChamba.Api.IAM.Infrastructure.Authentication;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AlguienDijoChamba.Api.Customers.Application.Queries;

public class CustomerLoginQueryHandler(
    IUserRepository userRepository,
    ICustomerRepository customerRepository,
    ICustomerJwtProvider customerJwtProvider,
    IPasswordHasher<User> passwordHasher
) : IRequestHandler<CustomerLoginQuery, string>
{
    public async Task<string> Handle(CustomerLoginQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken)
                   ?? throw new Exception("Usuario no encontrado.");

        var customer = await customerRepository.GetByUserIdAsync(user.Id, cancellationToken)
                       ?? throw new Exception("No es un cliente.");

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Contraseña incorrecta.");

        return customerJwtProvider.Generate(user, "Customer");
    }
}