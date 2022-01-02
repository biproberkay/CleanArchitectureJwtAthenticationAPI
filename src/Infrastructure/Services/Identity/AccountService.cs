using Application.Interfaces.Services.Identity;
using Shared.Communicate.Identity.Account;
using Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Localization;
using Infrastructure.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Shared.Communicate;
using Hangfire;
using Application.Interfaces.Services;
using Application.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Identity
{
    public class AccountService : IAccountService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly IStringLocalizer _localizer;
        private readonly IMailService _mailService;
        private readonly ApplicationSettings _settings;

        public AccountService(ITokenService tokenService, IMailService mailService, 
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
            //IStringLocalizer localizer, 
            IOptions<ApplicationSettings> settings)
        {
            _tokenService = tokenService;
            _mailService = mailService;
            _userManager = userManager;
            _roleManager = roleManager;
            //_localizer = localizer;
            _settings = settings.Value;
        }

        public async Task<IResult<LoginResponse>> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null)
                return await Result<LoginResponse>.FailAsync("User Not Found.");

            if (!user.IsActive)
                return await Result<LoginResponse>.FailAsync("User Not Active. Please contact the administrator.");
            
            if (!user.EmailConfirmed)
                return await Result<LoginResponse>.FailAsync("E-Mail not confirmed.");
            
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!passwordValid)
                return await Result<LoginResponse>.FailAsync("Invalid Credentials.");
            
            var claims = await GetClaimsAsync(user);
            var response = new LoginResponse()
            {
                Token = _tokenService.GenerateJwt(claims, _settings.JwtSettings)
            };
            return await Result<LoginResponse>.SuccessAsync(response);
        }
        private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var thisRole = await _roleManager.FindByNameAsync(role);
                var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
                permissionClaims.AddRange(allPermissionsForThisRoles);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

            return claims;
        }

        public async Task<IResult> Register(RegisterRequest request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                return await Result.FailAsync(string.Format("This Username: {0} has been taken.", request.UserName));
            }
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                IsActive = request.ActivateUser,
                EmailConfirmed = request.AutoConfirmEmail
            };

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
                if (userWithSamePhoneNumber != null)
                {
                    return await Result.FailAsync(string.Format("Phone number {0} is already registered.", request.PhoneNumber));
                }
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "RoleConstants.BasicRole");
                    if (!request.AutoConfirmEmail)
                    {
                        var verificationUri = await SendVerificationEmail(user, origin);
                        var mailRequest = new MailRequest
                        {
                            From = "mail@codewithmukesh.com",
                            To = user.Email,
                            Body = string.Format("Please confirm your account by <a href='{0}'>clicking here</a>.", verificationUri),
                            Subject = "Confirm Registration"
                        };
                        BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
                        return await Result<string>.SuccessAsync(user.Id, string.Format("User {0} Registered. Please check your Mailbox to verify!", user.UserName));
                    }
                    return await Result<string>.SuccessAsync(user.Id, string.Format("User {0} Registered.", user.UserName));
                }
                else
                {
                    return await Result.FailAsync(result.Errors.Select(a => a.Description.ToString()).ToList());
                }
            }
            else
            {
                return await Result.FailAsync(string.Format("Email {0} is already registered.", request.Email));
            }
        }

        private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
        {
            origin = "localhost:5000";
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/identity/user/confirm-email/";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            return verificationUri;
        }
        public string Logout()
        {
            throw new NotImplementedException();
        }

        public string ResetPasswordAsync()
        {
            throw new NotImplementedException();
        }

        public string VerifyPassword()
        {
            throw new NotImplementedException();
        }
        public string ConfirmEmailAsync()
        {
            throw new NotImplementedException();
        }

        public string ExternalLogin()
        {
            throw new NotImplementedException();
        }

        public string ForgotPasswordAsync()
        {
            throw new NotImplementedException();
        }

    }
}
