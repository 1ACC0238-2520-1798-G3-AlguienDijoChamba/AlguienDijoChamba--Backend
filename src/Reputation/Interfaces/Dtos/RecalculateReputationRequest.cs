namespace AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

public class RecalculateReputationRequest
{
    // Solo necesitamos el valor de la nueva reseña (ej. 4.5 estrellas)
    // El ProfessionalId viene de la URL del Controller.
    public double NewRatingValue { get; set; }
}