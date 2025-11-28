using AlguienDijoChamba.Api.IAM.Application.Commands;
using AlguienDijoChamba.Api.IAM.Application.Queries;
using AlguienDijoChamba.Api.IAM.Interfaces.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace AlguienDijoChamba.Api.IAM.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
public class IamController(ISender sender) : ControllerBase 
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
    
    // 🔍 DEBUG: Ver qué hay en el token
    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);
    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
    
    Console.WriteLine($"🔍 JWT DEBUG - NameIdentifier Claim: {userIdClaim?.Value}");
    Console.WriteLine($"🔍 JWT DEBUG - Todos los claims: {string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}={c.Value}"))}");
    
    string? userId = userIdClaim?.Value;
    
    return Ok(new LoginResponse(token, userId));
}
    
    [Authorize]
    [HttpDelete("delete-account")]
    public async Task<IActionResult> DeleteAccount(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new DeleteAccountCommand(userId);
        var result = await sender.Send(command, cancellationToken);

        if (result)
        {
            return NoContent(); 
        }
        
        return StatusCode(500, new { Message = "Error al eliminar la cuenta." });
    }
}
