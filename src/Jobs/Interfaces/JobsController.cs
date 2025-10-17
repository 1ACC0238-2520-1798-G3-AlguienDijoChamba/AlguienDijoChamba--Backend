using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlguienDijoChamba.Api.Jobs.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
public class JobsController : ControllerBase
{
    // Endpoint para que un cliente cree una solicitud
    [Authorize]
    [HttpPost("request")]
    public async Task<IActionResult> CreateJobRequest(/* [FromBody] CreateJobRequestDto request */)
    {
        // Aquí iría la lógica con MediatR para crear la solicitud
        // 1. Obtener el ID del cliente desde el token.
        // 2. Crear un CreateJobRequestCommand.
        // 3. Enviar el comando con MediatR.
        // 4. El handler crearía la entidad JobRequest y la guardaría.

        return Ok(new { message = "Endpoint de creación de solicitud listo para implementar." });
    }

    // Endpoint para que un profesional vea las solicitudes disponibles
    [Authorize]
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableRequests()
    {
        // Aquí iría la lógica con MediatR para obtener las solicitudes
        // 1. Crear un GetAvailableJobsQuery.
        // 2. El handler consultaría la base de datos por solicitudes con estado "Pending".

        return Ok(new { message = "Endpoint de solicitudes disponibles listo para implementar." });
    }
}