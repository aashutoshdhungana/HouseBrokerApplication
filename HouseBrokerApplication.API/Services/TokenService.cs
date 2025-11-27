using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Application.Constants;
using HouseBrokerApplication.Domain.Aggregates.UserInfo;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HouseBrokerApplication.API.Services
{
    public class TokenService(
        IConfiguration configuration
    ) : ITokenService
    {
        public string GenerateToken(UserInfo user, string clientId, string scopes, bool isBrokerLogin)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim("client_id", clientId),
                new Claim("scopes", scopes),
                new Claim(ClaimTypes.Role, (isBrokerLogin ? AppRoles.BROKER : AppRoles.HOUSESEEKER))
            };

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(30),
                claims: authClaims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
