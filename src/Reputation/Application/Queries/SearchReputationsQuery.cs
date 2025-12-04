// Archivo: Application/Queries/SearchReputationsQuery.cs (¡CREAR ESTE ARCHIVO!)

using System.Collections.Generic;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries;

// Implementar IRequest<List<UserReputationTechnicianDto>> o el tipo de lista que devuelvas
public record SearchReputationsQuery(
    string? searchTerm, 
    string? professionalIds, 
    int page, 
    int limit
) : IRequest<object>; // Usa el tipo de retorno real de tu lista de reputaciones