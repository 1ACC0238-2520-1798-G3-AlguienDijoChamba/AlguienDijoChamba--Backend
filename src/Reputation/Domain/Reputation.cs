namespace AlguienDijoChamba.Api.Reputation.Domain;

public class UserReputationTechnician
{
    public Guid Id { get; private set; }
    public Guid ProfessionalId { get; private set; }
    public double Rating { get; private set; }
    public int ReviewsCount { get; private set; }
    public string Level { get; private set; } = string.Empty;
    public decimal HourlyRate { get; private set; }

    private UserReputationTechnician() { }

    public UserReputationTechnician(Guid professionalId, double rating, int reviewsCount, string level, decimal hourlyRate)
    {
        Id = Guid.NewGuid();
        ProfessionalId = professionalId;
        Rating = rating;
        ReviewsCount = reviewsCount;
        Level = level;
        HourlyRate = hourlyRate;
    }

    public void UpdateRating(double rating, int reviewsCount)
    {
        Rating = rating;
        ReviewsCount = reviewsCount;
    }
    
    public void UpdateDetails(double rating, int reviewsCount, string level, decimal hourlyRate)
    {
        // Puedes reutilizar tu método UpdateRating
        UpdateRating(rating, reviewsCount);
        
        // Actualiza el resto de las propiedades
        Level = level;
        HourlyRate = hourlyRate;
    }
    
    public void UpdateAll(double rating, int reviewsCount, string level, decimal hourlyRate)
    {
        // Reutilizamos el método existente y actualizamos las propiedades restantes.
        UpdateRating(rating, reviewsCount); 
        
        Level = level;
        HourlyRate = hourlyRate;
    }
}