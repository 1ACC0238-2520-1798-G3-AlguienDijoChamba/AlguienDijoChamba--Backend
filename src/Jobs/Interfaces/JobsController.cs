using AlguienDijoChamba.Api.Jobs.Application.Commands; // ✅ Necesario para los comandos
using AlguienDijoChamba.Api.Jobs.Domain;
using AlguienDijoChamba.Api.Jobs.Interfaces.Dtos;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AlguienDijoChamba.Api.Jobs.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
public class JobsController : ControllerBase
{
    private readonly ISender _sender; // ✅ Usamos MediatR
    private readonly IJobRequestRepository _jobRequestRepository; // Solo para lecturas (GET)

    public JobsController(ISender sender, IJobRequestRepository jobRequestRepository)
    {
        _sender = sender;
        _jobRequestRepository = jobRequestRepository;
    }

    // ===========================================
    // 🔵 CREAR SOLICITUD (Flutter -> Pay -> Backend)
    // ===========================================
    [Authorize]
    [HttpPost("request")]
    public async Task<IActionResult> CreateJobRequest([FromBody] CreateActiveJobRequest request)
    {
        try
        {
            // 1. Obtener ID del cliente del token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var clientId))
            {
                return Unauthorized(new { message = "Invalid or missing client id in token" });
            }

            // 2. Crear el Comando
            var command = new CreateJobRequestCommand(
                CustomerId: clientId, // Usamos el ID del token
                ProfessionalId: request.ProfessionalId,
                Specialty: request.Specialty,
                Description: request.Description,
                Address: request.Address,
                ScheduledDate: request.ScheduledDate,
                ScheduledHour: request.ScheduledHour,
                AdditionalMessage: request.AdditionalMessage,
                Categories: request.Categories,
                PaymentMethod: request.PaymentMethod,
                TotalCost: request.TotalCost
            );

            // 3. Enviar al Handler (Aquí se guarda en BD y se notifica por SignalR)
            var jobDto = await _sender.Send(command);

            // 4. Retornar
            return CreatedAtAction(nameof(GetJobById), new { jobId = jobDto.Id }, jobDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating job request", error = ex.Message });
        }
    }

    // ===========================================
    // 🔵 CREAR ACTIVE JOB (Mismo endpoint lógico)
    // ===========================================
    [Authorize]
    [HttpPost("active")]
    public async Task<IActionResult> CreateActiveJob([FromBody] CreateActiveJobRequest request)
    {
        // Reutilizamos la lógica del comando, ya que es lo mismo: crear un trabajo y notificar.
        return await CreateJobRequest(request);
    }

    // ===========================================
    // 🔵 MÉTODOS DE LECTURA Y ACTUALIZACIÓN (GET/PATCH)
    // ===========================================

    [Authorize]
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveJobsForCurrentClient()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var clientId))
                return Unauthorized();

            var clientJobs = await _jobRequestRepository.GetByClientAsync(clientId);
            
            // Mapeo simple a DTO
            var response = clientJobs.Select(j => new JobDto
            {
                Id = j.Id,
                ClientId = j.ClientId,
                ProfessionalId = j.ProfessionalId ?? Guid.Empty,
                ProfessionalName = j.Professional?.FullName ?? "",
                Specialty = j.Specialty,
                Description = j.Description,
                Address = j.Address,
                ScheduledDate = j.ScheduledDate,
                ScheduledHour = j.ScheduledHour,
                AdditionalMessage = j.AdditionalMessage,
                Categories = j.Categories,
                PaymentMethod = j.PaymentMethod,
                TotalCost = j.TotalCost,
                Status = j.Status.ToString(),
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching active jobs", error = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("active/customer/{clientId}")]
    public async Task<IActionResult> GetActiveJobByClient(Guid clientId)
    {
        var job = await _jobRequestRepository.GetActiveJobByClientAsync(clientId);
        if (job == null) return NotFound(new { message = "No active job found" });
        
        // Mapeo manual rápido (o usar AutoMapper si lo tienes)
        return Ok(new JobDto { 
            Id = job.Id, 
            ClientId = job.ClientId,
            ProfessionalId = job.ProfessionalId ?? Guid.Empty,
            Status = job.Status.ToString(),
            TotalCost = job.TotalCost,
            // ... mapear resto de campos si es necesario
        });
    }

    [Authorize]
    [HttpGet("{jobId}")]
    public async Task<IActionResult> GetJobById(Guid jobId)
    {
        var job = await _jobRequestRepository.GetByIdAsync(jobId);
        if (job == null) return NotFound();

        return Ok(new JobDto
        {
            Id = job.Id,
            ClientId = job.ClientId,
            ProfessionalId = job.ProfessionalId ?? Guid.Empty,
            Specialty = job.Specialty,
            Description = job.Description,
            Address = job.Address,
            ScheduledDate = job.ScheduledDate,
            ScheduledHour = job.ScheduledHour,
            AdditionalMessage = job.AdditionalMessage,
            Categories = job.Categories,
            PaymentMethod = job.PaymentMethod,
            TotalCost = job.TotalCost,
            Status = job.Status.ToString(),
            CreatedAt = job.CreatedAt,
            UpdatedAt = job.UpdatedAt
        });
    }

    [Authorize]
    [HttpPatch("{jobId}/status")]
    public async Task<IActionResult> UpdateJobStatus(Guid jobId, [FromBody] UpdateJobStatusRequest request)
    {
        try
        {
            var job = await _jobRequestRepository.GetByIdAsync(jobId);
            if (job == null) return NotFound();

            if (Enum.TryParse<JobRequestStatus>(request.Status, out var newStatus))
            {
                job.UpdateStatus(newStatus);
                await _jobRequestRepository.UpdateAsync(job);
                return Ok(new { message = "Status updated", status = newStatus.ToString() });
            }
            return BadRequest("Invalid status");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating status", error = ex.Message });
        }
    }
}