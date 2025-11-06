namespace AlguienDijoChamba.Api.Reputation.Domain;

public class UserReputationTechnician
{
    public Guid Id { get; private set; }
    public Guid ProfessionalId { get; private set; }
    public double StarRating { get; private set; } 
    public int CompletedJobs { get; private set; } 

    public string ProfessionalLevel { get; private set; } = string.Empty;
    
    public decimal HourlyRate { get; private set; }

    private UserReputationTechnician() { }

    public UserReputationTechnician(Guid professionalId, double starRating, int completedJobs, string professionalLevel, decimal hourlyRate)
    {
        Id = Guid.NewGuid();
        ProfessionalId = professionalId;
        StarRating = starRating; 
        CompletedJobs = completedJobs;
        ProfessionalLevel = professionalLevel;
        HourlyRate = hourlyRate;
    }

    // Métodos de actualización deben usar los nuevos nombres
    public void UpdateRating(double starRating, int completedJobs)
    {
        StarRating = starRating;
        CompletedJobs = completedJobs;
    }
    
    public void UpdateDetails(double starRating, int completedJobs, string professionalLevel, decimal hourlyRate)
    {
        UpdateRating(starRating, completedJobs);
        
        // ⬅️ Propiedades actualizadas
        ProfessionalLevel = professionalLevel;
        HourlyRate = hourlyRate;
    }
    
    public void UpdateAll(double starRating, int completedJobs, string professionalLevel, decimal hourlyRate)
    {
        UpdateRating(starRating, completedJobs); 
        
        ProfessionalLevel = professionalLevel;
        HourlyRate = hourlyRate;
    }
}