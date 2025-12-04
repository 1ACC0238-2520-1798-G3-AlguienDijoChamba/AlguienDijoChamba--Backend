namespace AlguienDijoChamba.Api.Jobs.Interfaces.Dtos;

public class CreateActiveJobRequest
{
    public Guid ProfessionalId { get; set; }
    public Guid CustomerId { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string ScheduledHour { get; set; } = string.Empty;
    public string? AdditionalMessage { get; set; }
    public List<string> Categories { get; set; } = new();
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
}
