namespace AlguienDijoChamba.Api.Jobs.Interfaces.Dtos;

public class UpdateJobStatusRequest
{
    public string Status { get; set; } = string.Empty; // "Accepted", "Completed", "Declined"
}
