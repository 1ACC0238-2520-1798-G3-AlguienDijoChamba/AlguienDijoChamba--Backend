// --- Importaciones necesarias ---
using AlguienDijoChamba.Api.Jobs.Domain;
using AlguienDijoChamba.Api.Jobs.Interfaces.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR; // 🚀 Necesario para ISender
using AlguienDijoChamba.Api.Jobs.Application.Commands; // 🚀 Necesario para los comandos

namespace AlguienDijoChamba.Api.Jobs.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
public class JobsController : ControllerBase
{
    // --- Inyectar MediatR (ISender) y el Repositorio (para los GETs) ---
    private readonly ISender _sender;
    private readonly IJobRequestRepository _jobRequestRepository;

    public JobsController(ISender sender, IJobRequestRepository jobRequestRepository)
    {
        _sender = sender;
        _jobRequestRepository = jobRequestRepository;
    }

    // ===========================================
    // 🔵 ENDPOINT MODIFICADO (Usado por Cliente/Flutter)
    // ===========================================

    /// <summary>
    /// Endpoint para que un cliente cree una solicitud de trabajo.
    /// Esto guarda el Job y notifica a los Técnicos (Android) vía SignalR.
    /// </summary>
    [Authorize]
    [HttpPost("request")]
    public async Task<IActionResult> CreateJobRequest([FromBody] CreateActiveJobRequest request)
    {
        try
        {
            // 1. Creamos el Comando CQRS con los datos del DTO
            var command = new CreateJobRequestCommand(
                CustomerId: request.CustomerId,
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

            // 2. Enviamos a MediatR.
            // El CreateJobRequestCommandHandler se encargará de
            // guardar en BD y notificar a SignalR.
            var jobDto = await _sender.Send(command);

            // 3. Devolvemos 201 Created con el DTO del Job
            return CreatedAtAction(nameof(GetJobById), new { jobId = jobDto.Id }, jobDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating job request", error = ex.Message });
        }
    }

    // ===========================================
    // 🔵 ENDPOINTS EXISTENTES (Sin cambios)
    // ===========================================

    /// <summary>
    /// Endpoint para que un profesional vea las solicitudes disponibles
    /// </summary>
    [Authorize]
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableRequests()
    {
        try
        {
            // (Lógica futura de MediatR para GetAvailableJobsQuery)
            await Task.CompletedTask;
            return Ok(new { message = "Endpoint de solicitudes disponibles listo para implementar." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching available jobs", error = ex.Message });
        }
    }

    /// <summary>
    /// GET /api/v1/jobs/active
    /// (Este endpoint parece duplicado con 'request', pero lo mantenemos si es usado)
    /// </summary>
    [Authorize]
    [HttpPost("active")]
    public async Task<IActionResult> CreateActiveJob([FromBody] CreateActiveJobRequest request)
    {
        // Este método ahora es redundante si "request" hace lo mismo,
        // pero lo mantenemos por compatibilidad. Llama al mismo comando.
        return await CreateJobRequest(request);
    }

    /// <summary>
    /// GET /api/v1/jobs/active/customer/{clientId}
    /// Obtener el Active Job actual del cliente
    /// </summary>
    [Authorize]
    [HttpGet("active/customer/{clientId}")]
    public async Task<IActionResult> GetActiveJobByClient(Guid clientId)
    {
        try
        {
            var jobRequest = await _jobRequestRepository.GetActiveJobByClientAsync(clientId);
            if (jobRequest == null)
                return NotFound(new { message = "No active job found for this client" });

            var response = new JobDto
            {
                Id = jobRequest.Id,
                ClientId = jobRequest.ClientId,
                ProfessionalId = jobRequest.ProfessionalId ?? Guid.Empty,
                Specialty = jobRequest.Specialty,
                Description = jobRequest.Description,
                Address = jobRequest.Address,
                ScheduledDate = jobRequest.ScheduledDate,
                ScheduledHour = jobRequest.ScheduledHour,
                AdditionalMessage = jobRequest.AdditionalMessage,
                Categories = jobRequest.Categories,
                PaymentMethod = jobRequest.PaymentMethod,
                TotalCost = jobRequest.TotalCost,
                Status = jobRequest.Status.ToString(),
                CreatedAt = jobRequest.CreatedAt,
                UpdatedAt = jobRequest.UpdatedAt
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching active job", error = ex.Message });
        }
    }

    /// <summary>
    /// PATCH /api/v1/jobs/{jobId}/status
    /// Actualizar el estado del job (Completed, Declined, etc)
    /// </summary>
    [Authorize]
    [HttpPatch("{jobId}/status")]
    public async Task<IActionResult> UpdateJobStatus(Guid jobId, [FromBody] UpdateJobStatusRequest request)
    {
        // Nota: Este endpoint es llamado por el Cliente (Flutter) para Cancelar o Completar,
        // pero el Técnico (Android) usa el Hub de SignalR (RespondToRequest).
        try
        {
            var jobRequest = await _jobRequestRepository.GetByIdAsync(jobId);
            
            if (jobRequest == null)
            {
                return NotFound(new { message = "Job not found" });
            }

            if (!Enum.TryParse<JobRequestStatus>(request.Status, true, out var newStatus))
            {
                return BadRequest(new { message = "Invalid status" });
            }

            jobRequest.UpdateStatus(newStatus);
            await _jobRequestRepository.UpdateAsync(jobRequest); // UpdateAsync guarda los cambios
            
            return Ok(new
            {
                message = "Job status updated successfully",
                job = new { jobRequest.Id, Status = jobRequest.Status.ToString(), jobRequest.UpdatedAt }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating job status", error = ex.Message });
        }
    }

    /// <summary>
    /// GET /api/v1/jobs/{jobId}
    /// Obtener detalles de un job específico
    /// </summary>
    [Authorize]
    [HttpGet("{jobId}")]
    public async Task<IActionResult> GetJobById(Guid jobId)
    {
        try
        {
            var jobRequest = await _jobRequestRepository.GetByIdAsync(jobId);
            if (jobRequest == null)
                return NotFound(new { message = "Job not found" });

            return Ok(new JobDto
            {
                Id = jobRequest.Id,
                ClientId = jobRequest.ClientId,
                ProfessionalId = jobRequest.ProfessionalId ?? Guid.Empty,
                Specialty = jobRequest.Specialty,
                Description = jobRequest.Description,
                Address = jobRequest.Address,
                ScheduledDate = jobRequest.ScheduledDate,
                ScheduledHour = jobRequest.ScheduledHour,
                AdditionalMessage = jobRequest.AdditionalMessage,
                Categories = jobRequest.Categories,
                PaymentMethod = jobRequest.PaymentMethod,
                TotalCost = jobRequest.TotalCost,
                Status = jobRequest.Status.ToString(),
                CreatedAt = jobRequest.CreatedAt,
                UpdatedAt = jobRequest.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching job", error = ex.Message });
        }
    }
}