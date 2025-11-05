namespace AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

public class UpdateReputationRequest
{
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }
    public string Level { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
}