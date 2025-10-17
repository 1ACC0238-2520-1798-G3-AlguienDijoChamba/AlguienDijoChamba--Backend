using AlguienDijoChamba.Api.IAM.Application;
using AlguienDijoChamba.Api.IAM.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AlguienDijoChamba.Api.IAM.Application.Queries;

public class LoginQueryHandler(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IJwtProvider jwtProvider)
    : IRequestHandler<LoginQuery, string> // Implementa la interfaz de MediatR
{
    public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        // 1. Buscar al usuario por su email
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            throw new Exception("Credenciales inválidas.");
        }

        // 2. Verificar la contraseña
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new Exception("Credenciales inválidas.");
        }

        // 3. Generar y devolver el token JWT
        var token = jwtProvider.Generate(user);
        return token;
    }
}