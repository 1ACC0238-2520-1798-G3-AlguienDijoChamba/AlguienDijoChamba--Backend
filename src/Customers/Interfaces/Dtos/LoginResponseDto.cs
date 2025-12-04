namespace AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

public class LoginResponseDto
{
    // El Token JWT que ya estabas devolviendo
    public string Token { get; set; }
    
    // El Customer.Id que necesitas en Flutter para la sesión
    public Guid CustomerId { get; set; } 
}