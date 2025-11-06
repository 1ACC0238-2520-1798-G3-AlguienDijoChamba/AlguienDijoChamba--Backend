using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AlguienDijoChamba.Api.Reputation.Application.Commands;
using AlguienDijoChamba.Api.Reputation.Application.Queries;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssignTagsToProfessionalCommand = AlguienDijoChamba.Api.Reputation.Application.Commands.AssignTagsToProfessionalCommand;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

namespace AlguienDijoChamba.Api.Reputation.Interfaces;

[Authorize]
[ApiController]
[Route("api/v1/reputation")]
public class ReputationController(ISender sender) : ControllerBase 
{
    [AllowAnonymous] 
    [HttpGet] 
// 🚀 CORRECCIÓN: Aceptar AMBOS (el ID simple y el DTO de búsqueda)
    public async Task<IActionResult> GetReputation(
        [FromQuery] Guid? professionalId, 
        [FromQuery] SearchReputationsRequest request, // 👈 ¡ESTO ES LO QUE FALTA!
        CancellationToken cancellationToken)
    {
        // 1. CASO DE PRIORIDAD: Búsqueda de UN SOLO profesional (Usando el ID simple)
        if (professionalId.HasValue && professionalId.Value != Guid.Empty) 
        {
            var singleQuery = new GetReputationByProfessionalIdQuery(professionalId.Value);
            var response = await sender.Send(singleQuery, cancellationToken);
        
            if (response is null) return NotFound(); 
            // Se devuelve envuelto en una lista para evitar el error de formato del frontend.
            return Ok(new List<object> { response }); 
        }

        // 2. CASO GENERAL O COMBINADA (Ahora 'request' ya existe y tiene los campos)

        if (string.IsNullOrWhiteSpace(request.Search) && string.IsNullOrWhiteSpace(request.ProfessionalIds))        {
            // ... Lógica para devolver TODOS ...
            var allQuery = new GetAllReputationsQuery(); 
            var allResponse = await sender.Send(allQuery, cancellationToken); 
            return Ok(allResponse);
        }

        var searchQuery = new SearchReputationsQuery(
            searchTerm: request.Search,        // <-- Corregido de searchTerm a Search
            professionalIds: request.ProfessionalIds, // <-- Corregido de professionalIds a ProfessionalIds
            page: request.Page,                // <-- Corregido de page a Page
            limit: request.Limit               // <-- Corregido de limit a Limit
        );

        var responseList = await sender.Send(searchQuery, cancellationToken);
        return Ok(responseList);
    }
    
    // --- 2. POST (Creación Inicial de Reputación) ---
    [HttpPost("initial")] 
    public async Task<IActionResult> CreateInitialReputation(
        [FromBody] CreateInitialReputationRequest request, // DTO de Request
        CancellationToken cancellationToken)
    {
        if (request == null) return BadRequest("Request body cannot be null.");

        var command = new CreateInitialReputationCommand(
            request.ProfessionalId,
            request.InitialHourlyRate 
        );
        
        // ⚠️ Nota: Asumo que tu handler devuelve el objeto UserReputationTechnician
        var reputationEntity = await sender.Send(command, cancellationToken);
        
        return CreatedAtAction(nameof(GetReputation), new { professionalId = reputationEntity.ProfessionalId }, reputationEntity);
    }

    // --- 3. PUT (Actualización / Recálculo por Reseña) ---
    [HttpPut("{professionalId}/recalculate")] 
    public async Task<IActionResult> RecalculateReputation(
        Guid professionalId, 
        [FromBody] RecalculateReputationRequest request, // DTO que contiene solo NewRatingValue
        CancellationToken cancellationToken)
    {
        if (request == null) return BadRequest("Request body cannot be null.");

        var command = new RecalculateReputationCommand(
            professionalId,
            request.NewRatingValue
        );
        
        var reputationEntity = await sender.Send(command, cancellationToken);
        
        return Ok(reputationEntity);
    }
    
    // ----------------------------------------------------
    // ## 🛠️ Endpoints de Tags (Commands)
    // ----------------------------------------------------

    [Authorize]
    [HttpPost("tags")]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagCommand command, CancellationToken cancellationToken)
    {
        var tagDto = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTagById), new { tagId = tagDto.Id }, tagDto);
    }
    
    [Authorize]
    [HttpPut("tags/assign")]
    public async Task<IActionResult> AssignTagsToProfessional([FromBody] AssignTagsToProfessionalCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        
        if (!result) return BadRequest("Error al asignar las etiquetas.");
        
        return Ok("Etiquetas asignadas correctamente.");
    }
    
    // ----------------------------------------------------
    // ## 🔍 Endpoints de Tags (Queries)
    // ----------------------------------------------------

    [AllowAnonymous] 
    [HttpGet("tags")] 
    public async Task<IActionResult> GetAllTags(CancellationToken cancellationToken)
    {
        var query = new GetAllTagsQuery(); 
        var response = await sender.Send(query, cancellationToken);
        
        return Ok(response); 
    }
    
    [AllowAnonymous] 
    [HttpGet("tags/{tagId}")]
    public async Task<IActionResult> GetTagById(Guid tagId, CancellationToken cancellationToken)
    {
        // 🚀 CORRECCIÓN: Se debe declarar la variable de respuesta
        object? response = await sender.Send(new GetTagByIdQuery(tagId), cancellationToken);

        if (response is null) return NotFound();
        return Ok(response);
    }
    
    [AllowAnonymous] 
    [HttpGet("tags/{tagId}/professionals")] // ¡Nueva y clara ruta!
    public async Task<IActionResult> GetProfessionalsByTag(Guid tagId, CancellationToken cancellationToken)
    {
        // Se envía la nueva Query al sender (MediatR)
        var response = await sender.Send(new GetProfessionalsByTagQuery(tagId), cancellationToken);

        // Si la respuesta es una lista vacía, se puede decidir retornar Ok (200) con lista vacía,
        // o NotFound (404) si se considera que el "recurso" de profesionales asociados no existe.
        if (response == null || !response.Any())
        {
            // Opción: Retornar 404 si no hay profesionales asociados.
            // return NotFound(); 
        
            // Opción Recomendada: Retornar 200 OK con una lista vacía [], ya que el TagId *existe*.
            return Ok(Enumerable.Empty<Guid>());
        }
    
        // Retornar la lista de GUIDs (los ProfessionalId)
        return Ok(response); 
    }

    [AllowAnonymous] 
    [HttpGet("tags/professional/{professionalId}")] 
    public async Task<IActionResult> GetTagsByProfessionalId(Guid professionalId, CancellationToken cancellationToken)
    {
        var query = new GetTagsByProfessionalIdQuery(professionalId);
        var response = await sender.Send(query, cancellationToken);
        
        return Ok(response ?? new List<TagDto>()); 
    }
}