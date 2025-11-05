using System;
using AlguienDijoChamba.Api.Reputation.Domain;

namespace AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

// DTO (Data Transfer Object) para exponer la información de reputación
public class ReputationDto
{
    public Guid Id { get; set; }

    // Id del profesional al que pertenece esta reputación
    public Guid ProfessionalId { get; set; } 

    // Puntuación promedio (ej: 4.5)
    public double Rating { get; set; }
    // Número total de reseñas
    public int ReviewsCount { get; set; } 

    // Nivel del profesional (ej: "Junior", "Mid", "Senior")
    public string Level { get; set; } 

    // Tarifa por hora del profesional (si se gestiona aquí)
    public decimal HourlyRate { get; set; } 
}