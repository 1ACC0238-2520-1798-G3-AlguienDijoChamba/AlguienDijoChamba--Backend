using AlguienDijoChamba.Api.IAM.Application.Commands;
using AlguienDijoChamba.Api.IAM.Application.Queries;
using AlguienDijoChamba.Api.IAM.Interfaces.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Necesario para obtener el UserId del token
using Microsoft.AspNetCore.Authorization; // Necesario para proteger el endpoint

namespace AlguienDijoChamba.Api.IAM.Interfaces;

[ApiController]
[Route("api/v1/[controller]")] // La ruta será /api/v1/iam
public class IamController(ISender sender) : ControllerBase 
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(request.Email, request.Password, request.Dni, request.Nombres, request.Apellidos, request.Celular);
        var userId = await sender.Send(command, cancellationToken);
        return Ok(new { UserId = userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var query = new LoginQuery(request.Email, request.Password);
        var token = await sender.Send(query, cancellationToken);
        return Ok(new LoginResponse(token));
    }
    
    // --- NUEVO ENDPOINT PARA ELIMINAR CUENTA ---
    [Authorize] // Solo usuarios con un token válido pueden eliminar su cuenta
    [HttpDelete("delete-account")] // La ruta será DELETE /api/v1/iam/delete-account
    public async Task<IActionResult> DeleteAccount(CancellationToken cancellationToken)
    {
        // 1. Obtener el ID del usuario desde el token JWT (que es el usuario que hace la petición)
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // 2. Crear y enviar el comando de eliminación
        var command = new DeleteAccountCommand(userId);
        var result = await sender.Send(command, cancellationToken);

        // 3. Devolver respuesta
        if (result)
        {
            // 204 No Content es la respuesta estándar para una eliminación exitosa sin cuerpo.
            return NoContent(); 
        }
        
        // Si el resultado es falso, indicamos un error de servidor.
        return StatusCode(500, new { Message = "Error al eliminar la cuenta." });
    }
    // ---------------------------------------------
}