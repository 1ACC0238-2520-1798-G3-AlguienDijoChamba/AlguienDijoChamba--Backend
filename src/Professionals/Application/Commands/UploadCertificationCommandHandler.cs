// En: AlguienDijoChamba.Api.Professionals.Application.Commands/UploadCertificationCommandHandler.cs

using MediatR;
using Microsoft.AspNetCore.Hosting;
using System.IO; 
// using AlguienDijoChamba.Api.Professionals.Domain; <-- ESTA IMPORTACIÓN YA NO ES NECESARIA AQUÍ

namespace AlguienDijoChamba.Api.Professionals.Application.Commands;

public class UploadCertificationCommandHandler(IWebHostEnvironment webHostEnvironment) 
    : IRequestHandler<UploadCertificationCommand, string> // <-- CORRECCIÓN 1: La respuesta debe ser string
{
    // --- CORRECCIÓN 2: El tipo de retorno debe ser string ---
    public async Task<string> Handle(UploadCertificationCommand request, CancellationToken cancellationToken)
    {
        var uploadsFolder = Path.Combine(webHostEnvironment.ContentRootPath, "uploads", "certifications");

        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
        
        var uniqueFileName = $"{Guid.NewGuid()}_{request.File.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        // Devuelve la ruta relativa que la aplicación cliente usará
        var relativePath = $"/uploads/certifications/{uniqueFileName}";
        return relativePath; // <-- Asegura que se devuelve el string
    }
}