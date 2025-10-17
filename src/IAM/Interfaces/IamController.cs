using AlguienDijoChamba.Api.IAM.Application.Commands;
using AlguienDijoChamba.Api.IAM.Application.Queries;
using AlguienDijoChamba.Api.IAM.Interfaces.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlguienDijoChamba.Api.IAM.Interfaces;

[ApiController]
[Route("api/v1/[controller]")] // Esto usará el nombre "iam" para la ruta
public class IamController(ISender sender) : ControllerBase // <-- La clase se llama IamController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(request.Email, request.Password, request.Dni, request.Nombres, request.Apellidos, request.Celular);
        var userId = await sender.Send(command, cancellationToken);
        return Ok(new { UserId = userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var query = new LoginQuery(request.Email, request.Password);
        var token = await sender.Send(query, cancellationToken);
        return Ok(new LoginResponse(token));
    }
}