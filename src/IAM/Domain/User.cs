﻿namespace AlguienDijoChamba.Api.IAM.Domain;
public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    
    // Constructor requerido por EF Core
    private User() { }
    
    private User(Guid id, string email, string passwordHash) { Id = id; Email = email; PasswordHash = passwordHash; }
    public static User Create(string email, string passwordHash) => new(Guid.NewGuid(), email, passwordHash);
}