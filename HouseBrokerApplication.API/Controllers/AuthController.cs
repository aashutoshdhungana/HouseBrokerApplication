using HouseBrokerApplication.API.Extensions;
using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Application.DTOs.Requests;
using HouseBrokerApplication.Domain.Aggregates.UserInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseBrokerApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserIdentityService userIdentityService) : ControllerBase
    {
        [HttpPost("/api/register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserReq request)
        {
            var result = await userIdentityService.RegisterUserAsync(request);
            return result.HttpResponse();
        }

        [HttpPost("/api/login")]
        public async Task<IActionResult> Login([FromBody] LoginReq request)
        {
            var result = await userIdentityService.LoginAsync(request);
            return result.HttpResponse();
        }

        [HttpPost("/api/register/{profileType}")]
        [Authorize]
        public async Task<IActionResult> RegisterProfile([FromRoute] UserRoleType profileType)
        {
            var result = await userIdentityService.RegisterProfileAsync(profileType == UserRoleType.Broker);
            return result.HttpResponse();
        }
    }
}
