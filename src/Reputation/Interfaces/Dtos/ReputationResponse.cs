namespace AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

public class ReputationResponse
{
    public Guid ProfessionalId { get; set; }
    public double StarRating { get; set; }
    public int CompletedJobs { get; set; }
    public string ProfessionalLevel { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }

    public ReputationResponse(Guid professionalId, double starRating, int completedJobs, string professionalLevel, decimal hourlyRate)
    {
        ProfessionalId = professionalId;
        StarRating = starRating;
        CompletedJobs = completedJobs;
        ProfessionalLevel = professionalLevel;
        HourlyRate = hourlyRate;
    }
}