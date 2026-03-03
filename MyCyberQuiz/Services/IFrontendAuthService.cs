using MyCyberQuiz.Shared.DTOs;

namespace MyCyberQuiz.UI.Services
{
    public interface IFrontendAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task LogoutAsync();
    }
}
