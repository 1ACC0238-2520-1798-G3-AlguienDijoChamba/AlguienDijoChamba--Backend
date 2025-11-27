﻿using AlguienDijoChamba.Api.Professionals.Domain;

namespace AlguienDijoChamba.Api.Jobs.Domain;

// ✅ CORREGIDO: Agregué Cancelled
public enum JobRequestStatus { Pending, Accepted, Declined, Completed, Cancelled }

public class JobRequest
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; } // ID del usuario que solicita
    public Guid? ProfessionalId { get; private set; } // ID del profesional (opcional al inicio)

    // 👇 navegación al profesional
    public Professional? Professional { get; private set; }

    public string Specialty { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public JobRequestStatus Status { get; private set; }

    // ✨ NUEVOS CAMPOS AGREGADOS para Active Jobs
    public string Address { get; private set; } = string.Empty;
    public DateTime ScheduledDate { get; private set; }
    public string ScheduledHour { get; private set; } = string.Empty;
    public string? AdditionalMessage { get; private set; }
    public List<string> Categories { get; private set; } = new();
    public string PaymentMethod { get; private set; } = string.Empty;
    public decimal TotalCost { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Constructor requerido por EF Core
    private JobRequest() { }

    // ✨ NUEVO CONSTRUCTOR para crear Active Jobs
    public static JobRequest CreateActiveJob(
        Guid clientId,
        Guid professionalId,
        string specialty,
        string description,
        string address,
        DateTime scheduledDate,
        string scheduledHour,
        string? additionalMessage,
        List<string> categories,
        string paymentMethod,
        decimal totalCost)
    {
        return new JobRequest
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            ProfessionalId = professionalId,
            Specialty = specialty,
            Description = description,
            Address = address,
            ScheduledDate = scheduledDate,
            ScheduledHour = scheduledHour,
            AdditionalMessage = additionalMessage,
            Categories = categories,
            PaymentMethod = paymentMethod,
            TotalCost = totalCost,
            Status = JobRequestStatus.Accepted, // Active job = Accepted
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // ✨ NUEVO MÉTODO para actualizar estado
    public void UpdateStatus(JobRequestStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}
