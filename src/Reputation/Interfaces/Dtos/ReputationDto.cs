using System;
using AlguienDijoChamba.Api.Reputation.Domain;

namespace AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

// DTO (Data Transfer Object) para exponer la información de reputación
public class ReputationDto
{
    public Guid Id { get; set; }

    // Id del profesional al que pertenece esta reputación
    public Guid ProfessionalId { get; set; } 

    public double StarRating { get; set; }
    public int CompletedJobs { get; set; } 

    public string ProfessionalLevel { get; set; } 

    // Tarifa por hora del profesional (si se gestiona aquí)
    public decimal HourlyRate { get; set; } 
}