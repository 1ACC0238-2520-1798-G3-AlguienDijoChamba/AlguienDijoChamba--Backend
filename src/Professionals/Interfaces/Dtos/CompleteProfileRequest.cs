namespace AlguienDijoChamba.Api.Professionals.Interfaces.Dtos;

public record CompleteProfileRequest(
    int YearsOfExperience,
    decimal? HourlyRate,
    string ProfessionalBio
);