using System.Security.Claims;
using AlguienDijoChamba.Api.Professionals.Application.Queries;
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Interfaces.Dtos;
using AlguienDijoChamba.Api.Professionals.Application.Commands;
using AlguienDijoChamba.Api.Professionals.Interfaces.Dtos;
using Microsoft.AspNetCore.Authorization;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// Nota: He limpiado las importaciones duplicadas que tenías
namespace AlguienDijoChamba.Api.Professionals.Interfaces;

[ApiController]
[Route("api/v1/[controller]")] // Esto crea la ruta /api/v1/professionals
public class ProfessionalsController(ISender sender, IWebHostEnvironment webHostEnvironment) : ControllerBase
{
    [HttpGet("reniec/{dni}")]
    [ProducesResponseType(typeof(ReniecInfo), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> GetReniecInfo(string dni, CancellationToken cancellationToken)
    {
        var query = new GetReniecInfoQuery(dni);
        var result = await sender.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound(new ErrorResponse("DNI no encontrado."));
        }

        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("complete-profile")]
    // --- ESTE ES EL MÉTODO CORREGIDO ---
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // --- CORRECCIÓN AQUÍ ---
        // Ahora pasamos los 6 argumentos que el comando espera,
        // tomándolos del 'request' (DTO).
        var command = new CompleteProfileCommand(
            userId, 
            request.YearsOfExperience, 
            request.HourlyRate, 
            request.ProfessionalBio,
            request.ProfilePhotoUrl,     // <-- Argumento 5 (nuevo)
            request.CertificationUrls    // <-- Argumento 6 (nuevo)
        );
        
        var result = await sender.Send(command, cancellationToken);
        return Ok(new { Success = result });
    }
    
    [Authorize]
    [HttpPost("upload-photo")]
    public async Task<IActionResult> UploadProfilePhoto(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ErrorResponse("No se ha seleccionado ningún archivo."));

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new UploadProfilePhotoCommand(userId, file);
        var relativePath = await sender.Send(command, cancellationToken);

        var photoUrl = $"{Request.Scheme}://{Request.Host}{relativePath}";

        return Ok(new { FileUrl = photoUrl });
    }
    
    [Authorize] // Protegido por JWT
    [HttpGet("my-profile")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetMyProfileQuery(userId);
        var result = await sender.Send(query, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
    
    [Authorize]
    [HttpPost("upload-certification")] // <-- NUEVO ENDPOINT
    public async Task<IActionResult> UploadCertification(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ErrorResponse("No se ha seleccionado ningún archivo."));

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new UploadCertificationCommand(userId, file);
        var relativePath = await sender.Send(command, cancellationToken);
        
        var fileUrl = $"{Request.Scheme}://{Request.Host}{relativePath}";
        return Ok(new { FileUrl = fileUrl });
    }
}