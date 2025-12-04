namespace AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;

public class CreateInitialReputationRequest
{
    public Guid ProfessionalId { get; set; }
    public decimal InitialHourlyRate { get; set; }
}