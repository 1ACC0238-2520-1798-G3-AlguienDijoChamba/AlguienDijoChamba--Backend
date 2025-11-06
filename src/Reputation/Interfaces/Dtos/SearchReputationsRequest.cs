// Archivo: Interfaces/Dtos/SearchReputationsRequest.cs (Crear este archivo)

namespace AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

public class SearchReputationsRequest
{
    // Mapea el parámetro 'search' de la URL (ej: ?search=Rafael)
    public string? Search { get; set; } 
    
    // Mapea el parámetro 'professionalIds' de la URL (ej: ?professionalIds=id1,id2)
    public string? ProfessionalIds { get; set; } 
    public string Nombres { get; set; } = string.Empty; // ¡Frontend espera esto!
    public string Apellidos { get; set; } = string.Empty;
    
    // Paginación
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}