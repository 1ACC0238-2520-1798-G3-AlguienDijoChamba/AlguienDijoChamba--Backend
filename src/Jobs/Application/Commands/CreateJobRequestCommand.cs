using AlguienDijoChamba.Api.Jobs.Interfaces.Dtos; // Para JobDto
using MediatR;
using System;
using System.Collections.Generic;

namespace AlguienDijoChamba.Api.Jobs.Application.Commands;

public record CreateJobRequestCommand(
    Guid CustomerId,
    Guid ProfessionalId,
    string Specialty,
    string Description,
    string Address,
    DateTime ScheduledDate,
    string ScheduledHour,
    string? AdditionalMessage,
    List<string> Categories,
    string PaymentMethod,
    decimal TotalCost
) : IRequest<JobDto>; // Devuelve el DTO del Job creado