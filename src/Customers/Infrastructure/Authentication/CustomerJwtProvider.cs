using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlguienDijoChamba.Api.Customers.Application;
using AlguienDijoChamba.Api.IAM.Domain;
using AlguienDijoChamba.Api.IAM.Infrastructure.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AlguienDijoChamba.Api.Customers.Infrastructure.Authentication;

public class CustomerJwtProvider(IOptions<JwtOptions> options) : ICustomerJwtProvider
{
    private readonly JwtOptions _options = options.Value;
    
    public string Generate(User user, string role)
    {
        // Método nuevo para clientes / otros roles
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, role) // <- agrega el rol al token
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}