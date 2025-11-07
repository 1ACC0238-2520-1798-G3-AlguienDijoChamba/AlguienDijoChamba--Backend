namespace AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

public class CreateJobReviewReputationRequest
{
    public Guid JobId { get; set; }
    public int Rating { get; set; }
    public string Review { get; set; }
}
