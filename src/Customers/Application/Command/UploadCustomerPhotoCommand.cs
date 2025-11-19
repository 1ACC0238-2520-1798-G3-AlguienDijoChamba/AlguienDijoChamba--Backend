using MediatR;
using Microsoft.AspNetCore.Http; // Necesario para IFormFile

namespace AlguienDijoChamba.Api.Customers.Application.Command;

// COMANDO para subir la foto de perfil
public record UploadCustomerPhotoCommand(
    Guid UserId,     // Para identificar a qué cliente pertenece la foto
    IFormFile PhotoFile // El archivo de la imagen en sí
) : IRequest<string>; // ⚠️ Devolverá la URL pública de la foto subida