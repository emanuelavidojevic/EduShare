using EduShareBackend.Api.Models;

namespace EduShareBackend.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(UserRegisterDTO registerDto);
        Task<TokenDTO> LoginAsync(UserLoginDTO loginDto);
        Task<TokenDTO> RefreshAccessTokenAsync(string refreshToken);
        void ClearCookies();
    }
}