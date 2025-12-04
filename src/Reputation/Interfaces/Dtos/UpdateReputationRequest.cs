namespace AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

public class UpdateReputationRequest
{
    public double StarRating { get; set; }
    public int CompletedJobs { get; set; }
    public string ProfessionalLevel { get; set; } = string.Empty;
    
    public decimal HourlyRate { get; set; }
}