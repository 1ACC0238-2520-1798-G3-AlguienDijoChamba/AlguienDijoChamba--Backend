namespace AlguienDijoChamba.Api.Professionals.Domain;

public class Professional
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Dni { get; private set; } = string.Empty;
    public string Nombres { get; private set; } = string.Empty;
    public string Apellidos { get; private set; } = string.Empty;
    public string Celular { get; private set; } = string.Empty;

    // Campos que pueden ser opcionales o se añaden más tarde
    public string? ProfilePhotoUrl { get; private set; }
    public int YearsOfExperience { get; private set; }
    public decimal? HourlyRate { get; private set; }
    public string ProfessionalBio { get; private set; } = string.Empty;
    
    // --- CAMPOS FALTANTES QUE CAUSAN EL ERROR ---
    public string Ocupacion { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty; // Email de contacto
    public DateTime? FechaNacimiento { get; private set; }
    public string? Genero { get; private set; }
    
    // Campo para guardar las URLs de certificaciones (JSON string)
    public string? CertificationUrls { get; private set; }
    
    // Constructor requerido por EF Core (privado y vacío)
    private Professional() { }

    private Professional(Guid userId, string dni, string nombres, string apellidos, string celular, string email){
        Id = Guid.NewGuid();
        UserId = userId;
        Dni = dni;
        Nombres = nombres;
        Apellidos = apellidos;
        Celular = celular;
        Email = email; // Inicializa Email de contacto con el email de registro
        ProfessionalBio = string.Empty;
        Ocupacion = string.Empty;
    }
    
    public static Professional Create(Guid userId, string dni, string nombres, string apellidos, string celular, string email)
        => new(userId, dni, nombres, apellidos, celular, email);
    
    // --- MÉTODOS DE ACTUALIZACIÓN ---
    
    // Método completo que usan los comandos de "Completar Perfil"
    public void UpdateProfile(
        int yearsOfExperience, 
        decimal? hourlyRate, 
        string professionalBio, 
        string? profilePhotoUrl,
        string? certificationUrlsJson
    )
    {
        YearsOfExperience = yearsOfExperience;
        HourlyRate = hourlyRate;
        ProfessionalBio = professionalBio;
        CertificationUrls = certificationUrlsJson;
        
        if (!string.IsNullOrEmpty(profilePhotoUrl))
        {
            ProfilePhotoUrl = profilePhotoUrl;
        }
    }

    public void UpdateProfilePhoto(string photoUrl)
    {
        ProfilePhotoUrl = photoUrl;
    }

    // Método que usan los comandos de "Actualizar Detalles" (ProfileScreen)
    public void UpdateDetails(string email, string celular, string ocupacion, DateTime? fechaNacimiento, string? genero)
    {
        Email = email;
        Celular = celular;
        Ocupacion = ocupacion;
        FechaNacimiento = fechaNacimiento;
        Genero = genero;
    }
}