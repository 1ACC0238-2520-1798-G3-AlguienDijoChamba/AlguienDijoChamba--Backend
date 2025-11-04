// En: AlguienDijoChamba.Api.Professionals.Application.Commands/UploadProfilePhotoCommandHandler.cs

using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System.IO; // Importación necesaria

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

        // --- CORRECCIÓN AQUÍ: Usamos ContentRootPath ---
        var uploadsFolder = Path.Combine(webHostEnvironment.ContentRootPath, "uploads", "profile-photos");
        
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
        
        var uniqueFileName = $"{Guid.NewGuid()}_{request.File.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        // Ruta relativa que la aplicación cliente usará para cargar la imagen
        var relativePath = $"/uploads/profile-photos/{uniqueFileName}";

        professional.UpdateProfilePhoto(relativePath);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return relativePath;
    }
}