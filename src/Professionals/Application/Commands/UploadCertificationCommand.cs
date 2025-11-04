// En: AlguienDijoChamba.Api.Professionals.Application.Commands/UploadCertificationCommand.cs
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AlguienDijoChamba.Api.Professionals.Application.Commands;

// Devuelve la URL relativa de la certificación subida
public record UploadCertificationCommand(Guid UserId, IFormFile File) : IRequest<string>;