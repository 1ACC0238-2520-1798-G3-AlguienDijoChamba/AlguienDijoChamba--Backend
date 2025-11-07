namespace AlguienDijoChamba.Api.Jobs.Interfaces.Dtos;

public class ActiveJobResponse
{
    public JobDto Job { get; set; } = new();
    public ProfessionalDto Professional { get; set; } = new();
}

public class JobDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid ProfessionalId { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string ScheduledHour { get; set; } = string.Empty;
    public string? AdditionalMessage { get; set; }
    public List<string> Categories { get; set; } = new();
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ProfessionalDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public decimal HourlyRate { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? ProfessionalLevel { get; set; }
    public double StarRating { get; set; }
    public List<string> Specialties { get; set; } = new();
}
