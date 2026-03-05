using MyCyberQuiz.Shared.DTOs;

namespace MyCyberQuiz.UI.Services.Interfaces
{
    public interface IFrontendProfileService
    {
        // Hämtar statistik och historik
        Task<UserProfileDto?> GetProfileAsync();

        // Metoder för inställningar
        Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordDto dto);
        Task<AuthResponseDto> ChangeEmailAsync(ChangeEmailDto dto);
    }
}
