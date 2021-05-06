using System.Threading.Tasks;
using testtest.dto;

using testtestdbcontext.testtest.Models;

namespace testtest.Service
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateUser(AuthenticationReqest ar);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
        Task<AuthenticationResult> AuthenticateGuest(GuestUser ar);
    }
}