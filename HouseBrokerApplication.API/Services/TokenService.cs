using HouseBrokerApplication.API.Configurations.ConfigModels;
using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Application.Constants;
using HouseBrokerApplication.Domain.Aggregates.UserInfo;
using HouseBrokerApplication.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HouseBrokerApplication.API.Services
{
    public class TokenService(
        IConfiguration configuration,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IOptions<List<ClientSetting>> clientSettings
    ) : ITokenService
    {
        public async Task<Result<(UserInfo, string)>> GenerateToken(string username, string password, string clientId, string clientSecret, bool isBrokerLogin)
        {
            var identityUser = await userManager.Users.Include(x => x.UserInfo).FirstOrDefaultAsync(user => user.UserName == username);
            if (identityUser is null)
                return Result<(UserInfo, string)>.Failure("Either username or password doesnot match");

            var singnInResult = await signInManager.CheckPasswordSignInAsync(identityUser, password, false);
            if (!singnInResult.Succeeded)
            {
                return Result<(UserInfo, string)>.Failure("Either username or password doesnot match");
            }

            if (isBrokerLogin && !identityUser.IsBroker)
                return Result<(UserInfo, string)>.Failure("User is not registered as broker");

            if (!isBrokerLogin && !identityUser.IsHomeSeeker)
                return Result<(UserInfo, string)>.Failure("User is not registered as home seeker");

            var user = identityUser.UserInfo;
            var clients = clientSettings.Value;
            var clientSetting = clients.Where(x => x.ClientId == clientId && x.ClientSecret == clientSecret).FirstOrDefault();
            if (clientSetting is null)
                return Result<(UserInfo, string)>.Failure("Invalid client");

            var scopes = string.Join(',', clientSetting.AllowedScopes);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id.ToString()),
                new Claim("userinfo_id", user.Id.ToString()),
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

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Result<(UserInfo, string)>.Success("Logged in Successful", (user, tokenString));
        }
    }


}
