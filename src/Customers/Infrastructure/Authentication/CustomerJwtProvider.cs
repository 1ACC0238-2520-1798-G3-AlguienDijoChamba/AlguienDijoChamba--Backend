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
        // El Claim de rol se ha eliminado, dejando solo los Claims base (ID y Email).
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email)
            // Se quitó: new(ClaimTypes.Role, role)
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