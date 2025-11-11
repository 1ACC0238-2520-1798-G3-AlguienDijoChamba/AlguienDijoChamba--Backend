using System.Security.Claims;
using AlguienDijoChamba.Api.Professionals.Application.Queries;
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Interfaces.Dtos; 
using AlguienDijoChamba.Api.Professionals.Application.Commands;
using AlguienDijoChamba.Api.Professionals.Interfaces.Dtos; 
using Microsoft.AspNetCore.Authorization; 
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlguienDijoChamba.Api.Professionals.Interfaces;

[ApiController]
[Route("api/v1/[controller]")] // Define la ruta base /api/v1/professionals
public class ProfessionalsController(ISender sender, IWebHostEnvironment webHostEnvironment) : ControllerBase
{
    /// <summary>
    /// Obtiene información de RENIEC a partir de un número de DNI.
    /// </summary>
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
    
    [Authorize] // Protegido por JWT
    [HttpPost("complete-profile")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Pasamos los 6 argumentos que el comando espera, incluyendo las URLs de subida.
        var command = new CompleteProfileCommand(
            userId, 
            request.YearsOfExperience, 
            request.HourlyRate, 
            request.ProfessionalBio,
            request.ProfilePhotoUrl,     // Argumento 5: URL de la foto
            request.CertificationUrls    // Argumento 6: URLs de certificaciones
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

        // Construimos la URL completa aquí
        var photoUrl = $"{Request.Scheme}://{Request.Host}{relativePath}";

        return Ok(new { FileUrl = photoUrl });
    }
    
    [Authorize] // Protegido por JWT
    [HttpPost("upload-certification")] // Endpoint para subida de certificación
    public async Task<IActionResult> UploadCertification(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ErrorResponse("No se ha seleccionado ningún archivo."));

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new UploadCertificationCommand(userId, file);
        var relativePath = await sender.Send(command, cancellationToken);
        
        // Construimos la URL completa
        var fileUrl = $"{Request.Scheme}://{Request.Host}{relativePath}";
        return Ok(new { FileUrl = fileUrl });
    }
    
    [Authorize] // Protegido por JWT
    [HttpGet("my-profile")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetMyProfileQuery(userId);
        var result = await sender.Send(query, cancellationToken);
        // Devuelve 404 si el perfil no se encontró, lo cual el frontend debe manejar.
        return result is null ? NotFound() : Ok(result);    
    }
    
    [Authorize]
    [HttpPut("my-profile")] // <-- Usa el método PUT y la misma ruta
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Creamos el comando de actualización (debe coincidir con la definición del comando)
        var command = new UpdateProfileCommand(
            userId,
            request.Email,
            request.Celular,
            request.Ocupacion,
            request.FechaNacimiento,
            request.Genero
        );

        var result = await sender.Send(command, cancellationToken);
        
        // Si el resultado es true (actualización exitosa), devolvemos 204 No Content
        return result ? NoContent() : NotFound();
    }
}