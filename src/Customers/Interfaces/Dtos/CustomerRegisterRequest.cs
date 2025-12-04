namespace AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

public class CustomerRegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Celular { get; set; } = string.Empty;
}