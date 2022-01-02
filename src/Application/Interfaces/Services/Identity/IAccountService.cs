using Shared.Communicate.Identity.Account;
using Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Identity
{
    public interface IAccountService : IService
    {
        Task<IResult<LoginResponse>> Login(LoginRequest loginRequest);
        Task<IResult> Register(RegisterRequest registerRequest, string origin);
        string Logout();
        string ConfirmEmailAsync();
        string ExternalLogin();
        string VerifyPassword();
        string ForgotPasswordAsync();
        string ResetPasswordAsync();
    }
}
