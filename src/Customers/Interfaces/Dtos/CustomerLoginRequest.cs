namespace AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

public class CustomerLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}