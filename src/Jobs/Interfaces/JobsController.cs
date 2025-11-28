// src/Jobs/Interfaces/JobsController.cs

using AlguienDijoChamba.Api.Jobs.Domain;
using AlguienDijoChamba.Api.Jobs.Interfaces.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AlguienDijoChamba.Api.Jobs.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
public class JobsController : ControllerBase
{
    // --- Inyectar MediatR (ISender) y el Repositorio (para los GETs) ---
    private readonly ISender _sender;
    private readonly IJobRequestRepository _jobRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public JobsController(IJobRequestRepository jobRequestRepository, IUnitOfWork unitOfWork)
    {
        _sender = sender;
        _jobRequestRepository = jobRequestRepository;
    }

    // ===========================================
    // 🔵 ENDPOINT MODIFICADO (Usado por Cliente/Flutter)
    // ===========================================

    // Endpoint para que un cliente cree una solicitud
    [Authorize]
    [HttpPost("request")]
    public async Task<IActionResult> CreateJobRequest([FromBody] CreateActiveJobRequest request)
    {
        try
        {
            // ✅ 1. Obtener el ID del cliente desde el token (UserId real)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("sub");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var clientId))
            {
                return Unauthorized(new { message = "Invalid or missing client id in token" });
            }

            // 2. Crear la solicitud usando SIEMPRE el clientId del token
            var jobRequest = JobRequest.CreateActiveJob(
                clientId: clientId,
                professionalId: request.ProfessionalId,
                specialty: request.Specialty,
                description: request.Description,
                address: request.Address,
                scheduledDate: request.ScheduledDate,
                scheduledHour: request.ScheduledHour,
                additionalMessage: request.AdditionalMessage,
                categories: request.Categories,
                paymentMethod: request.PaymentMethod,
                totalCost: (decimal)request.TotalCost
            );

            _jobRequestRepository.Add(jobRequest);
            await _unitOfWork.SaveChangesAsync();
            // ✨ RETORNA EL JOB COMPLETO CON ID
            return CreatedAtAction(nameof(GetActiveJobByClient), new { clientId = clientId }, new JobDto
            {
                Id = jobRequest.Id,
                ClientId = jobRequest.ClientId,
                ProfessionalId = jobRequest.ProfessionalId ?? Guid.Empty,
                ProfessionalName = string.Empty, // opcional, o cargar luego
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
            return StatusCode(500, new { message = "Error creating job request", error = ex.Message });
        }
    }

    // Endpoint para que un profesional vea las solicitudes disponibles
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
    /// ✨ NUEVO: Obtener todos los Active Jobs del cliente autenticado
    /// </summary>
    [Authorize]
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveJobsForCurrentClient()
    {
        try
        {
            // 1. Obtener ClientId desde el token (claim "sub" o el que uses)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("sub");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var clientId))
            {
                return Unauthorized(new { message = "Invalid or missing client id in token" });
            }

            // 👇 LOG PARA VER QUÉ CLIENTID ESTÁ USANDO
            Console.WriteLine($"DEBUG JOBS ACTIVE: clientId desde token = {clientId}");

            // 2. Obtener jobs de ese cliente
            var clientJobs = await _jobRequestRepository.GetByClientAsync(clientId);

            // 3. Filtrar solo los que estén activos
            var activeJobs = clientJobs
                .Where(j => j.Status == JobRequestStatus.Accepted
                         || j.Status == JobRequestStatus.Completed
                         || j.Status == JobRequestStatus.Cancelled)
                .ToList();

            var response = activeJobs.Select(jobRequest => new JobDto
            {
                Id = jobRequest.Id,
                ClientId = jobRequest.ClientId,
                ProfessionalId = jobRequest.ProfessionalId ?? Guid.Empty,

                // ✨ NUEVO: tomar nombre del técnico desde navigation property
                ProfessionalName = jobRequest.Professional?.FullName ?? string.Empty,

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


            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching active jobs", error = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/v1/jobs/active
    /// ✨ NUEVO: Crear un nuevo Active Job (cuando el cliente paga)
    /// </summary>
    [Authorize]
    [HttpPost("active")]
    public async Task<IActionResult> CreateActiveJob([FromBody] CreateActiveJobRequest request)
    {
        try
        {
            // ✅ Obtener ClientId desde el token (ignoramos request.CustomerId)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("sub");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var clientId))
            {
                return Unauthorized(new { message = "Invalid or missing client id in token" });
            }

            // Validaciones
            if (string.IsNullOrEmpty(request.Address))
                return BadRequest(new { message = "Address is required" });

            if (request.ScheduledDate < DateTime.UtcNow.AddHours(1))
                return BadRequest(new { message = "Scheduled date must be at least 1 hour in the future" });

            if (request.TotalCost <= 0)
                return BadRequest(new { message = "Total cost must be greater than 0" });

            // Crear el job usando SIEMPRE el clientId del token
            var jobRequest = JobRequest.CreateActiveJob(
                clientId: clientId,
                professionalId: request.ProfessionalId,
                specialty: request.Specialty,
                description: request.Description,
                address: request.Address,
                scheduledDate: request.ScheduledDate,
                scheduledHour: request.ScheduledHour,
                additionalMessage: request.AdditionalMessage,
                categories: request.Categories,
                paymentMethod: request.PaymentMethod,
                totalCost: (decimal)request.TotalCost
            );

            _jobRequestRepository.Add(jobRequest);
            await _unitOfWork.SaveChangesAsync();

            // ✨ RETORNA EL JOB COMPLETO CON ID
            return CreatedAtAction(nameof(GetActiveJobByClient), new { clientId = clientId }, new JobDto
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
            return StatusCode(500, new { message = "Error creating active job", error = ex.Message });
        }
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
                ProfessionalName = jobRequest.Professional?.FullName ?? string.Empty,
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
        try
        {
            Console.WriteLine($"🔵 PATCH /status recibida - JobId: {jobId}, Status: {request.Status}");

            var jobRequest = await _jobRequestRepository.GetByIdAsync(jobId);

            if (jobRequest == null)
            {
                Console.WriteLine($"❌ Job no encontrado: {jobId}");
                return NotFound(new { message = "Job not found" });
            }

            Console.WriteLine($"✅ Job encontrado. Status actual: {jobRequest.Status}");

            if (!Enum.TryParse<JobRequestStatus>(request.Status, out var newStatus))
            {
                Console.WriteLine($"❌ Status inválido: {request.Status}");
                return BadRequest(new { message = "Invalid status" });
            }

            if (jobRequest.Status == newStatus)
            {
                Console.WriteLine($"❌ Job ya está en estado: {newStatus}");
                return BadRequest(new { message = $"Job is already {newStatus}" });
            }

            Console.WriteLine($"🔄 Actualizando status de {jobRequest.Status} a {newStatus}");
            jobRequest.UpdateStatus(newStatus);
            await _jobRequestRepository.UpdateAsync(jobRequest);

            Console.WriteLine($"✅ Job actualizado exitosamente");
            return Ok(new
            {
                message = "Job status updated successfully",
                job = new
                {
                    jobRequest.Id,
                    Status = jobRequest.Status.ToString(),
                    jobRequest.UpdatedAt
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ EXCEPCIÓN: {ex.Message}");
            Console.WriteLine($"❌ STACK: {ex.StackTrace}");
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
                ProfessionalName = jobRequest.Professional?.FullName ?? string.Empty,
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