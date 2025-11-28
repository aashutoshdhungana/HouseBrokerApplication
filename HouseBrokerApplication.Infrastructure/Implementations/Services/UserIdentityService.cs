using FluentValidation;
using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Application.DTOs.Requests;
using HouseBrokerApplication.Application.DTOs.Responses;
using HouseBrokerApplication.Domain.Aggregates.UserInfo;
using HouseBrokerApplication.Domain.Base;
using HouseBrokerApplication.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseBrokerApplication.Infrastructure.Implementations.Services
{
    public class UserIdentityService(
        UserManager<AppUser> userManager,
        IRepository<UserInfo> userInfoRepo,
        ITokenService tokenService,
        IValidator<RegisterUserReq> registerValidator,
        IValidator<LoginReq> loginValidator,
        ICurrentUserService currentUserService) : IUserIdentityService
    {
        public async Task<Result<UserInfo>> RegisterUserAsync(RegisterUserReq request)
        {
            var validationResult = registerValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result<UserInfo>.ValidationError(validationResult.Errors);
            }

            var existingUser = await userManager.Users.FirstOrDefaultAsync(x => x.Email == request.ContactEmail || x.PhoneNumber == request.ContactPhone);
            if (existingUser is not null)
            {
                string message = $"User already registered as {(request.RegisterAsBroker ? "broker" : "home seeker")}";
                var isRequestedProfileCreated = existingUser.IsBroker && request.RegisterAsBroker || existingUser.IsHomeSeeker && !request.RegisterAsBroker;
                if (isRequestedProfileCreated) return Result<UserInfo>.Failure(message);
                return Result<UserInfo>.Failure($"User already exists with the same email or phone number, please login and add {(request.RegisterAsBroker ? "broker" : "home sekeer")} profile");
            }

            var userInfo = new UserInfo(request.FirstName, request.LastName, request.ContactPhone, request.ContactEmail);
            userInfoRepo.Add(userInfo);

            var identityUser = new AppUser(request.UserName, userInfo);
            identityUser.IsBroker = request.RegisterAsBroker;
            identityUser.IsHomeSeeker = !request.RegisterAsBroker;
            identityUser.Email = request.ContactEmail;
            identityUser.PhoneNumber = request.ContactPhone;

            var creationResult = await userManager.CreateAsync(identityUser, request.Password);
            if (!creationResult.Succeeded)
                return Result<UserInfo>.Failure("Failed to create user profile");

            return Result<UserInfo>.Success("User profile created", userInfo);
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginReq request)
        {
            var validationResult = loginValidator.Validate(request);
            if (!validationResult.IsValid)
                return Result<LoginResponse>.ValidationError(validationResult.Errors);

            var loginResult = await tokenService.GenerateToken(
                request.UserName,
                request.Password,
                request.ClientId,
                request.ClientSecret,
                request.IsBrokerLogin
                );

            if (!loginResult.IsSuccess)
                return Result<LoginResponse>.Failure(loginResult.Message ?? "Failed to login");

            (UserInfo? userInfo, string token) = loginResult.Data;
            var loginResp = new LoginResponse()
            { UserInfo = userInfo, Token = token };

            return Result<LoginResponse>.Success("Successfull", loginResp);
        }

        public async Task<Result<string>> RegisterProfileAsync(bool isBrokerPrfile)
        {
            if (currentUserService.ClaimsPrincipal is null)
                return Result<string>.Failure("User is not logged in");

            var user = await userManager.GetUserAsync(currentUserService.ClaimsPrincipal);
            if (user == null) return Result<string>.Failure("User not found");

            if (isBrokerPrfile && user.IsBroker) return Result<string>.Failure("User already registered as broker");
            if (!isBrokerPrfile && user.IsHomeSeeker) return Result<string>.Failure("User already registed as home seeker");

            if (isBrokerPrfile) user.IsBroker = true;
            if (!isBrokerPrfile) user.IsHomeSeeker = true;
            user.SecurityStamp = Guid.NewGuid().ToString();
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result<string>.Failure("Failed to update the user profile");
            return Result<string>.Success("Successfull", string.Empty);
        }
    }
}
