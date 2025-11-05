using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AlguienDijoChamba.Api.Reputation.Application.Commands;
using AlguienDijoChamba.Api.Reputation.Application.Queries;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization; // Necesario para [Authorize]
using Microsoft.AspNetCore.Mvc;

namespace AlguienDijoChamba.Api.Reputation.Interfaces;

[Authorize]
[ApiController]
[Route("api/v1/reputation")]
public class ReputationController(ISender sender) : ControllerBase 
{
    // --- 1. GET (Query) ---
    // La ruta es fija: [HttpGet]. El ID es ahora un parámetro opcional de QUERY STRING.
    // Llama a:
    // 1. /api/v1/reputation (Obtiene todos)
    // 2. /api/v1/reputation?professionalId={guid} (Obtiene uno)
    [HttpGet] 
    public async Task<IActionResult> GetReputation([FromQuery] Guid? professionalId, CancellationToken cancellationToken)
    {
        // Si el ID tiene un valor (fue pasado en el query string)
        if (professionalId.HasValue && professionalId.Value != Guid.Empty)
        {
            // Lógica para UN SOLO profesional
            var query = new GetReputationByProfessionalIdQuery(professionalId.Value);
            var response = await sender.Send(query, cancellationToken);
        
            if (response is null) return NotFound();

            return Ok(response);
        }
        else
        {
            // Lógica para OBTENER TODOS los profesionales (si el ID es nulo)
            var query = new GetAllReputationsQuery(); 
            var response = await sender.Send(query, cancellationToken);
            
            return Ok(response);
        }
    }
    
    // --- 2. PUT Unificado (Command) ---
    // Este endpoint está protegido por el [Authorize] de la clase.
    [HttpPut("{professionalId}")] 
    public async Task<IActionResult> UpdateOrCreateReputation(
        Guid professionalId, 
        [FromBody] UpdateReputationRequest request, 
        CancellationToken cancellationToken)
    {
        if (request == null) return BadRequest("Request body cannot be null.");

        var command = new UpdateReputationCommand(
            professionalId,
            request.Rating,
            request.ReviewsCount,
            request.Level,
            request.HourlyRate
        );
        
        var reputationEntity = await sender.Send(command, cancellationToken);
        
        return Ok(reputationEntity);
    }
}