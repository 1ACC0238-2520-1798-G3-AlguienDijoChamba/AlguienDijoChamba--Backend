namespace AlguienDijoChamba.Api.Jobs.Domain;

public enum JobRequestStatus { Pending, Accepted, Declined, Completed }

public class JobRequest
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; } // ID del usuario que solicita
    public Guid? ProfessionalId { get; private set; } // ID del profesional (opcional al inicio)
    public string Specialty { get; private set; }
    public string Description { get; private set; }
    public JobRequestStatus Status { get; private set; }
    // ... otros campos como ubicación, fecha, etc.
}