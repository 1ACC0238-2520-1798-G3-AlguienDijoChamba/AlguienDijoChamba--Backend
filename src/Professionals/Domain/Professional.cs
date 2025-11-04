namespace AlguienDijoChamba.Api.Professionals.Domain;
public class Professional
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Dni { get; private set; }
    public string Nombres { get; private set; }
    public string Apellidos { get; private set; }
    public string Celular { get; private set; }
    public string Email { get; private set; }
    public DateTime? FechaNacimiento { get; private set; }
    public string? Genero { get; private set; }
    public string? Ocupacion { get; private set; }
    public string? ProfilePhotoUrl { get; private set; }
    public int YearsOfExperience { get; private set; }
    public decimal? HourlyRate { get; private set; }
    public string ProfessionalBio { get; private set; }
    private Professional(Guid userId, string dni, string nombres, string apellidos, string celular, string email){
        Id = Guid.NewGuid();
        UserId = userId;
        Dni = dni;
        Nombres = nombres;
        Apellidos = apellidos;
        Celular = celular;
        Email = email;
        ProfessionalBio = string.Empty; // Valor por defecto
    }
    public static Professional Create(Guid userId, string dni, string nombres, string apellidos, string celular, string email)
        => new(userId, dni, nombres, apellidos, celular, email);
    public void UpdateProfile(int yearsOfExperience, decimal? hourlyRate, string professionalBio)
    {
        YearsOfExperience = yearsOfExperience;
        HourlyRate = hourlyRate;
        ProfessionalBio = professionalBio;
    }

    public void UpdateAdditionalInfo(DateTime? fechaNacimiento, string? genero, string? ocupacion)
    {
        FechaNacimiento = fechaNacimiento;
        Genero = genero;
        Ocupacion = ocupacion;
    }

    public void UpdateProfilePhoto(string photoUrl)
    {
        ProfilePhotoUrl = photoUrl;
    }
}