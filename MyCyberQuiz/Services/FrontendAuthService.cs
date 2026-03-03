using Microsoft.AspNetCore.Components.Authorization;
using MyCyberQuiz.Shared.DTOs;
using MyCyberQuiz.UI.Auth;

namespace MyCyberQuiz.UI.Services
{
    public class FrontendAuthService : IFrontendAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public FrontendAuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Skicka e-post och lösenord till LoginAPI
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginDto);

                // Ta emot svaret från API
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                // Vid success, spara token-strängen i ProtectedSessionStorage
                if (result != null && result.IsSuccessful && !string.IsNullOrWhiteSpace(result.Token))
                {
                    var customProvider = (CustomAuthStateProvider)_authStateProvider;
                    await customProvider.MarkUserAsAuthenticated(result.Token);
                }

                return result ?? new AuthResponseDto(false, "Ett oväntat fel inträffade.", null);
            }
            catch (Exception ex)
            {
                return new AuthResponseDto(false, $"Kunde inte nå servern: {ex.Message}", null);
            }
        }

        public async Task LogoutAsync()
        {
            // Radera token-strängen i ProtectedSessionStorage
            var customProvider = (CustomAuthStateProvider)_authStateProvider;
            await customProvider.MarkUserAsLoggedOut();
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Skickar DTO:n till vår nya endpoint i API:et
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

            return result ?? new AuthResponseDto(false, "Ett oväntat fel uppstod.", null);
        }
    }
}
