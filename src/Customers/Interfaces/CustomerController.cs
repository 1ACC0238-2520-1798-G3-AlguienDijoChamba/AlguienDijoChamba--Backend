using AlguienDijoChamba.Api.Customers.Application.Command;
using AlguienDijoChamba.Api.Customers.Application.Queries;
using AlguienDijoChamba.Api.Customers.Interfaces.Dtos;
using AlguienDijoChamba.Api.IAM.Application.Commands;
using AlguienDijoChamba.Api.IAM.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlguienDijoChamba.Api.Customers.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
public class CustomerController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Registrar un cliente (Customer)
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CustomerRegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCustomerCommand(
            request.Email, request.Password, request.Nombres, request.Apellidos, request.Celular
        );
        var userId = await sender.Send(command, cancellationToken);
        return Ok(new { UserId = userId });
    }

    /// <summary>
    /// Login de un cliente (Customer)
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CustomerLoginRequest request, CancellationToken cancellationToken)
    {
        var query = new CustomerLoginQuery(request.Email, request.Password);
        
        // 🛑 CAMBIO CLAVE: El resultado es el DTO completo: { Token, CustomerId }
        var result = await sender.Send(query, cancellationToken);
        
        // 🛑 Devolvemos el DTO completo. El 404 de Flutter desaparecerá.
        return Ok(result); 
    }
}