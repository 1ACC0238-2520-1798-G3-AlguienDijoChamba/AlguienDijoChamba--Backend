using AlguienDijoChamba.Api.Jobs.Domain;
using MediatR;
using System;

namespace AlguienDijoChamba.Api.Jobs.Application.Commands;

public record UpdateJobStatusCommand(
    Guid JobId,
    JobRequestStatus NewStatus,
    Guid? ProfessionalId,
    decimal? ProposedCost // <-- 🚀 CAMPO AÑADIDO
) : IRequest;