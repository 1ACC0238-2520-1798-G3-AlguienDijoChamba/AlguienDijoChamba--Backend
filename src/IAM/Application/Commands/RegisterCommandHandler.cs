using AlguienDijoChamba.Api.IAM.Domain;
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AlguienDijoChamba.Api.IAM.Application.Commands;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IProfessionalRepository professionalRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher<User> passwordHasher)
    : IRequestHandler<RegisterCommand, Guid> // Implementa la interfaz de MediatR
{
    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que el email no exista
        if (await userRepository.GetByEmailAsync(request.Email, cancellationToken) is not null)
        {
            throw new Exception("El correo electrónico ya está en uso.");
        }

        // 2. Encriptar la contraseña
        var passwordHash = passwordHasher.HashPassword(null!, request.Password);

        // 3. Crear la entidad User
        var user = User.Create(request.Email, passwordHash);
        userRepository.Add(user);

        // 4. Crear la entidad Professional asociada
        var professional = Professional.Create(user.Id, request.Dni, request.Nombres, request.Apellidos, request.Celular);
        professionalRepository.Add(professional);

        // 5. Guardar todos los cambios en la base de datos
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Devolver el ID del nuevo usuario
        return user.Id;
    }
}