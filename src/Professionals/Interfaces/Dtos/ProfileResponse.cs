namespace AlguienDijoChamba.Api.Professionals.Interfaces.Dtos;

public record ProfileResponse(
    string UserName,
    string ProfessionalLevel,
    double StarRating,
    int CompletedJobs,
    decimal AvailableBalance
);