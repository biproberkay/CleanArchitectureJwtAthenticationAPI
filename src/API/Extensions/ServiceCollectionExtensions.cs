using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Application.Settings;
using Infrastructure.Data;
using Infrastructure.Models.Identity;
using Infrastructure.Services;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        => services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        internal static IServiceCollection AddIdentity(this IServiceCollection services, IdentitySettings identitySettings)
        {
            services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = identitySettings.PasswordRequiredLength;
                    options.Password.RequireDigit = identitySettings.PasswordRequireDigit;
                    options.Password.RequireLowercase = identitySettings.PasswordRequireLowercase;
                    options.Password.RequireNonAlphanumeric = identitySettings.PasswordRequireNonAlphanumeric;
                    options.Password.RequireUppercase = identitySettings.PasswordRequireUppercase;
                    options.User.RequireUniqueEmail = identitySettings.RequireUniqueEmail;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
        internal static IServiceCollection AddDeveloperServices(this IServiceCollection services)
        {
            return services
                .AddTransient<ITokenService,TokenService>()
                .AddTransient<IAccountService,AccountService>()
                .AddTransient<IMailService,SMTPMailService>();
        }
        internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new()
                    {
                        // The signing key must match!
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = false,
                        //ValidIssuers = jwtSettings.ValisIssuers,

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = false,
                        //ValidAudiences = jwtSettings.ValidAuidiences,

                        // Validate the token expiry
                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        // If you want to allow a certain amount of clock drift, set that here:
                        ClockSkew = TimeSpan.Zero,
                    };
                });
            return services;
        }
    }
}
