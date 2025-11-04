﻿namespace AlguienDijoChamba.Api.Professionals.Domain;
public class Professional
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Dni { get; private set; } = string.Empty;
    public string Nombres { get; private set; } = string.Empty;
    public string Apellidos { get; private set; } = string.Empty;
    public string Celular { get; private set; } = string.Empty;
    public string? ProfilePhotoUrl { get; private set; }
    public int YearsOfExperience { get; private set; }
    public decimal? HourlyRate { get; private set; }
    public string ProfessionalBio { get; private set; } = string.Empty;
    public string Ocupacion { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty; // El email de contacto, puede ser diferente al de login
    public DateTime? FechaNacimiento { get; private set; }
    public string? Genero { get; private set; }
    
    // Constructor requerido por EF Core
    private Professional() { }
    
    private Professional(Guid userId, string dni, string nombres, string apellidos, string celular, string email){
        Id = Guid.NewGuid();
        UserId = userId;
        Dni = dni;
        Nombres = nombres;
        Apellidos = apellidos;
        Celular = celular;
        Email = email;
        ProfessionalBio = string.Empty;
        Ocupacion = string.Empty;
    }
    public static Professional Create(Guid userId, string dni, string nombres, string apellidos, string celular, string email)
        => new(userId, dni, nombres, apellidos, celular, email);
    public void UpdateProfile(int yearsOfExperience, decimal? hourlyRate, string professionalBio)
    {
        YearsOfExperience = yearsOfExperience;
        HourlyRate = hourlyRate;
        ProfessionalBio = professionalBio;
    }

    public void UpdateProfilePhoto(string photoUrl)
    {
        ProfilePhotoUrl = photoUrl;
    }
    public void UpdateDetails(string email, string celular, string ocupacion, DateTime? fechaNacimiento, string? genero)
    {
        // Nota: Nombres y Apellidos NO se actualizan aquí,
        // respetando tu regla de negocio.
        Email = email;
        Celular = celular;
        Ocupacion = ocupacion;
        FechaNacimiento = fechaNacimiento;
        Genero = genero;
    }
}