namespace AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

public class ReputationResponse
{
    public Guid ProfessionalId { get; set; }
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }
    public string Level { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }

    public ReputationResponse(Guid professionalId, double rating, int reviewsCount, string level, decimal hourlyRate)
    {
        ProfessionalId = professionalId;
        Rating = rating;
        ReviewsCount = reviewsCount;
        Level = level;
        HourlyRate = hourlyRate;
    }
}