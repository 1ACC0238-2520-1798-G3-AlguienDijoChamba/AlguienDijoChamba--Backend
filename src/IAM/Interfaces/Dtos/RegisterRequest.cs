namespace AlguienDijoChamba.Api.IAM.Interfaces.Dtos;

public record RegisterRequest(
    string Email,
    string Password,
    string Dni,
    string Nombres,
    string Apellidos,
    string Celular);