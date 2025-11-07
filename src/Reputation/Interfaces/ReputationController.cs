using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AlguienDijoChamba.Api.Reputation.Application.Commands;
using AlguienDijoChamba.Api.Reputation.Application.Queries;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AssignTagsToProfessionalCommand = AlguienDijoChamba.Api.Reputation.Application.Commands.AssignTagsToProfessionalCommand;

namespace AlguienDijoChamba.Api.Reputation.Interfaces;

[Authorize]
[ApiController]
[Route("api/v1/reputation")]
public class ReputationController(ISender sender) : ControllerBase 
{
    // --- 1. GET (Búsqueda de Reputación) ---
    
    /// <summary>
    /// Recupera la reputación de uno o más profesionales.
    /// </summary>
    /// <remarks>
    /// Si se proporciona 'professionalId', devuelve solo la reputación de ese profesional.
    /// Si se usan los parámetros de búsqueda, realiza una búsqueda paginada o filtrada.
    /// Si no se proporciona ningún parámetro, devuelve todas las reputaciones.
    /// </remarks>
    /// <param name="professionalId">ID único del profesional (si se busca uno solo).</param>
    /// <param name="request">DTO para la búsqueda general de reputaciones (filtros y paginación).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Una lista de objetos de reputación (Response 200).</returns>
    [AllowAnonymous] 
    [HttpGet] 
    [ProducesResponseType(typeof(List<object>), 200)] // Tipo genérico
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetReputation(
        [FromQuery] Guid? professionalId, 
        [FromQuery] SearchReputationsRequest request,
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

        // 2. CASO GENERAL O COMBINADA 
        if (string.IsNullOrWhiteSpace(request.Search) && string.IsNullOrWhiteSpace(request.ProfessionalIds))
        {
            // ... Lógica para devolver TODOS ...
            var allQuery = new GetAllReputationsQuery(); 
            var allResponse = await sender.Send(allQuery, cancellationToken); 
            return Ok(allResponse);
        }

        var searchQuery = new SearchReputationsQuery(
            searchTerm: request.Search,
            professionalIds: request.ProfessionalIds,
            page: request.Page,
            limit: request.Limit
        );

        var responseList = await sender.Send(searchQuery, cancellationToken);
        return Ok(responseList);
    }
    
    // --- 2. POST (Creación Inicial de Reputación) ---
    
    /// <summary>
    /// Crea el registro inicial de reputación para un nuevo profesional.
    /// </summary>
    /// <remarks>
    /// Este endpoint debe llamarse al registrar un nuevo técnico. Establece la tarifa inicial.
    /// </remarks>
    /// <param name="request">DTO con el ProfessionalId y la InitialHourlyRate.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>La entidad de reputación creada.</returns>
    [HttpPost("initial")] 
    [ProducesResponseType(typeof(object), 201)] // Tipo genérico
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateInitialReputation(
        [FromBody] CreateInitialReputationRequest request,
        CancellationToken cancellationToken)
    {
        if (request == null) return BadRequest("Request body cannot be null.");

        var command = new CreateInitialReputationCommand(
            request.ProfessionalId,
            request.InitialHourlyRate 
        );
        
        // Asumo que tu handler devuelve una entidad o DTO de reputación
        var reputationEntity = await sender.Send(command, cancellationToken);
        
        // El DTO de reputación debe tener un ProfessionalId
        return CreatedAtAction(nameof(GetReputation), new { professionalId = GetProfessionalIdFromResponse(reputationEntity) }, reputationEntity);
    }
    
    // Función auxiliar para evitar errores de compilación si la entidad no es conocida
    private static Guid GetProfessionalIdFromResponse(object response)
    {
        // Esto es un placeholder; en un código real, harías un casting seguro o usarías interfaces.
        // Asumo que la propiedad existe en la entidad que devuelve el handler.
        var type = response.GetType();
        var prop = type.GetProperty("ProfessionalId");
        if (prop != null)
        {
            return (Guid)prop.GetValue(response)!;
        }
        return Guid.Empty; // Devuelve un GUID vacío si no se puede obtener la propiedad.
    }


    // --- 3. PUT (Actualización / Recálculo por Reseña) ---
    
    /// <summary>
    /// Recalcula la reputación de un profesional tras recibir una nueva reseña o rating.
    /// </summary>
    /// <param name="professionalId">ID único del profesional.</param>
    /// <param name="request">DTO que contiene el nuevo valor de rating a aplicar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>La entidad de reputación actualizada.</returns>
    [HttpPut("{professionalId}/recalculate")] 
    [ProducesResponseType(typeof(object), 200)] // Tipo genérico
    [ProducesResponseType(400)]
    public async Task<IActionResult> RecalculateReputation(
        Guid professionalId, 
        [FromBody] RecalculateReputationRequest request,
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

    /// <summary>
    /// Crea una nueva etiqueta de habilidad o servicio. Requiere autenticación.
    /// </summary>
    /// <param name="command">Comando con el nombre y descripción del Tag.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El DTO de la etiqueta creada.</returns>
    [Authorize]
    [HttpPost("tags")]
    [ProducesResponseType(typeof(TagDto), 201)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagCommand command, CancellationToken cancellationToken)
    {
        var tagDto = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTagById), new { tagId = tagDto.Id }, tagDto);
    }
    
    /// <summary>
    /// Asigna una o más etiquetas existentes a un profesional. Requiere autenticación.
    /// </summary>
    /// <param name="command">Comando con el ID del profesional y la lista de TagIds a asignar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Confirmación de asignación exitosa.</returns>
    [Authorize]
    [HttpPut("tags/assign")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> AssignTagsToProfessional([FromBody] AssignTagsToProfessionalCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        
        if (!result) return BadRequest("Error al asignar las etiquetas.");
        
        return Ok("Etiquetas asignadas correctamente.");
    }
    
    // ----------------------------------------------------
    // ## 🔍 Endpoints de Tags (Queries)
    // ----------------------------------------------------

    /// <summary>
    /// Obtiene la lista completa de todas las etiquetas disponibles.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Una lista de DTOs de etiquetas.</returns>
    [AllowAnonymous] 
    [HttpGet("tags")] 
    [ProducesResponseType(typeof(List<TagDto>), 200)]
    public async Task<IActionResult> GetAllTags(CancellationToken cancellationToken)
    {
        var query = new GetAllTagsQuery(); 
        var response = await sender.Send(query, cancellationToken);
        
        return Ok(response); 
    }
    
    /// <summary>
    /// Obtiene una etiqueta específica por su ID.
    /// </summary>
    /// <param name="tagId">ID de la etiqueta a buscar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El DTO de la etiqueta solicitada.</returns>
    [AllowAnonymous] 
    [HttpGet("tags/{tagId}")]
    [ProducesResponseType(typeof(TagDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTagById(Guid tagId, CancellationToken cancellationToken)
    {
        object? response = await sender.Send(new GetTagByIdQuery(tagId), cancellationToken);

        if (response is null) return NotFound();
        return Ok(response);
    }
    
    /// <summary>
    /// Obtiene una lista de IDs de profesionales asociados a una etiqueta específica.
    /// </summary>
    /// <remarks>
    /// Retorna una lista vacía si la etiqueta no tiene profesionales asociados, pero la etiqueta existe.
    /// </remarks>
    /// <param name="tagId">ID de la etiqueta.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de GUIDs (ProfessionalId).</returns>
    [AllowAnonymous] 
    [HttpGet("tags/{tagId}/professionals")]
    [ProducesResponseType(typeof(IEnumerable<Guid>), 200)]
    public async Task<IActionResult> GetProfessionalsByTag(Guid tagId, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetProfessionalsByTagQuery(tagId), cancellationToken);

        if (response == null || !response.Any())
        {
            return Ok(Enumerable.Empty<Guid>());
        }
    
        return Ok(response); 
    }

    /// <summary>
    /// Obtiene las etiquetas que han sido asignadas a un profesional específico.
    /// </summary>
    /// <param name="professionalId">ID del profesional.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de DTOs de las etiquetas asignadas.</returns>
    [AllowAnonymous] 
    [HttpGet("tags/professional/{professionalId}")] 
    [ProducesResponseType(typeof(List<TagDto>), 200)]
    public async Task<IActionResult> GetTagsByProfessionalId(Guid professionalId, CancellationToken cancellationToken)
    {
        var query = new GetTagsByProfessionalIdQuery(professionalId);
        var response = await sender.Send(query, cancellationToken);
        
        return Ok(response ?? new List<TagDto>()); 
    }
}