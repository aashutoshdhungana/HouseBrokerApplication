using FluentValidation;
using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Application.Constants;
using HouseBrokerApplication.Application.DTOs.Requests;
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
        AbstractValidator<RegisterUserReq> registerValidator,
        AbstractValidator<LoginReq> loginValidator) : IUserIdentityService
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
            identityUser.Email = request.ContactEmail;
            identityUser.PhoneNumber = request.ContactPhone;

            var creationResult = await userManager.CreateAsync(identityUser, request.Password);
            if (!creationResult.Succeeded)
                return Result<UserInfo>.Failure("Failed to create user profile");

            return Result<UserInfo>.Success("User profile created", userInfo);
        }

        public Task<Result<UserInfo>> Login(LoginReq request)
        {

        }
    }
}
