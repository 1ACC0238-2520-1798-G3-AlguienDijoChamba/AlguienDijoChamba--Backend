using MediatR;
using Microsoft.AspNetCore.Http;

namespace AlguienDijoChamba.Api.Professionals.Application.Commands;

// Este comando recibe el ID del usuario y el archivo, y devuelve la URL como un string.
public record UploadProfilePhotoCommand(Guid UserId, IFormFile File) : IRequest<string>;