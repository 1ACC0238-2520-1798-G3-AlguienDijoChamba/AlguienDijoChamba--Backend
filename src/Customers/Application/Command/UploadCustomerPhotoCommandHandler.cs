using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using AlguienDijoChamba.Api.Shared.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http; 
using System;
using System.Threading;
using System.Threading.Tasks;


namespace AlguienDijoChamba.Api.Customers.Application.Command;

public class UploadCustomerPhotoCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    // ⚠️ INYECTAMOS EL SERVICIO DE ALMACENAMIENTO DE ARCHIVOS
    IFileStorageService fileStorageService 
) : IRequestHandler<UploadCustomerPhotoCommand, string> 
{
    public async Task<string> Handle(UploadCustomerPhotoCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener la entidad Customer usando el ID PRIMARIO (Customer.Id)
        var customer = await customerRepository.GetByUserIdAsync(request.UserId, cancellationToken); // 👈 CORRECCIÓN A GetByIdAsync

        if (customer is null)
        {
            // El mensaje de error debe reflejar que no se encontró por el ID de la tabla
            throw new Exception($"Cliente con Id {request.UserId} no encontrado para subir foto."); 
        }

        // 2. Validar que se haya enviado un archivo
        if (request.PhotoFile == null || request.PhotoFile.Length == 0)
        {
            throw new ArgumentException("El archivo de la foto es inválido o está vacío.");
        }

        // 3. ☁️ Subir la foto al servicio de almacenamiento
        // Usamos el ID de la tabla como identificador único para el archivo (request.UserId)
        var photoUrl = await fileStorageService.UploadFileAsync(
            request.PhotoFile, 
            request.UserId.ToString(), // El ID pasado en el Command es el Customer.Id
            cancellationToken
        );
        
        // 4. Actualizar la URL de la foto en la entidad de dominio
        customer.UpdatePhotoUrl(photoUrl); 
        
        // 5. Persistir los cambios en la base de datos
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // 6. Devolver la URL pública de la foto
        return photoUrl;
    }
}