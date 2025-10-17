using System.Security.Claims;
using AlguienDijoChamba.Api.Professionals.Application.Queries;
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Shared.Interfaces.Dtos; 
using AlguienDijoChamba.Api.Professionals.Application.Commands;
using AlguienDijoChamba.Api.Professionals.Interfaces.Dtos; 
using Microsoft.AspNetCore.Authorization; 
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using AlguienDijoChamba.Api.Professionals.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlguienDijoChamba.Api.Professionals.Interfaces;

[ApiController]
[Route("api/v1/[controller]")] // Esto crea la ruta /api/v1/professionals
public class ProfessionalsController(ISender sender, IWebHostEnvironment webHostEnvironment) : ControllerBase
{
    /// <summary>
    /// Obtiene información de RENIEC a partir de un número de DNI.
    /// </summary>
    /// <param name="dni">El número de DNI de 8 dígitos.</param>
    /// <returns>Los nombres y apellidos asociados al DNI.</returns>
    [HttpGet("reniec/{dni}")]
    [ProducesResponseType(typeof(ReniecInfo), 200)] // Ayuda a Swagger a saber qué esperar
    [ProducesResponseType(typeof(ErrorResponse), 404)] // Ahora Swagger sabe qué esperar en un 404
    public async Task<IActionResult> GetReniecInfo(string dni, CancellationToken cancellationToken)
    {
        var query = new GetReniecInfoQuery(dni);
        var result = await sender.Send(query, cancellationToken);

        // Si el servicio no devuelve nada, retornamos un 404 Not Found
        if (result is null)
        {
            return NotFound(new ErrorResponse("DNI no encontrado.")); // Devuelve el DTO de error
        }

        return Ok(result);
    }
    [Authorize] // Protege el endpoint, solo usuarios con token pueden acceder
    [HttpPost("complete-profile")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest request, CancellationToken cancellationToken)
    {
        // Obtenemos el ID del usuario desde el token JWT
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new CompleteProfileCommand(userId, request.YearsOfExperience, request.HourlyRate, request.ProfessionalBio);
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

        // Construimos la URL completa aquí, en el controlador
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
    
}