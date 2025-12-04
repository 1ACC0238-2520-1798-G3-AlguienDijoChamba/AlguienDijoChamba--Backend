using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.Customers.Interfaces.Dtos;
using AlguienDijoChamba.Api.IAM.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace AlguienDijoChamba.Api.Customers.Application.Queries;

public class CustomerLoginQueryHandler(
    IUserRepository userRepository,
    ICustomerRepository customerRepository,
    ICustomerJwtProvider customerJwtProvider,
    IPasswordHasher<User> passwordHasher
) : IRequestHandler<CustomerLoginQuery, LoginResponseDto> // 🛑 CAMBIADO AQUÍ
{
    // 🛑 CAMBIAMOS el retorno del Handle a LoginResponseDto 🛑
    public async Task<LoginResponseDto> Handle(CustomerLoginQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken)
                   ?? throw new Exception("Usuario no encontrado.");

        // Obtienes la entidad Customer. ¡Aquí está el ID que buscamos!
        var customer = await customerRepository.GetByUserIdAsync(user.Id, cancellationToken)
                       ?? throw new Exception("No es un cliente."); 

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Contraseña incorrecta.");

        // 1. Generar el Token
        var token = customerJwtProvider.Generate(user, "Customer");
        
        // 2. Devolver el DTO con ambos campos
        return new LoginResponseDto
        {
            Token = token,
            CustomerId = customer.Id // 🛑 ¡Obtenido de la entidad Customer!
        };
    }
}