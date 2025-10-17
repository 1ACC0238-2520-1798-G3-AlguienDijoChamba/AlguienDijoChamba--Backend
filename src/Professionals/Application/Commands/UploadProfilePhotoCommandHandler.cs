using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace AlguienDijoChamba.Api.Professionals.Application.Commands;

public class UploadProfilePhotoCommandHandler(
    IProfessionalRepository professionalRepository,
    IUnitOfWork unitOfWork,
    IWebHostEnvironment webHostEnvironment)
    : IRequestHandler<UploadProfilePhotoCommand, string>
{
    public async Task<string> Handle(UploadProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        var professional = await professionalRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (professional is null)
        {
            throw new Exception("Perfil de profesional no encontrado.");
        }

        var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "profile-photos");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{request.File.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        // Para la URL, necesitamos el contexto de la petición, que no tenemos aquí.
        // La mejor práctica es devolver una ruta relativa y construir la URL completa en el controlador.
        var relativePath = $"/uploads/profile-photos/{uniqueFileName}";

        professional.UpdateProfilePhoto(relativePath);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return relativePath;
    }
}