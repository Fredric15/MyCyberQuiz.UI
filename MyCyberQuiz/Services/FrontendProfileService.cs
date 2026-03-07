using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MyCyberQuiz.Shared.DTOs;
using MyCyberQuiz.UI.Services.Interfaces;
using System.Net.Http.Headers;

namespace MyCyberQuiz.UI.Services
{
    public class FrontendProfileService : IFrontendProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly ProtectedSessionStorage _sessionStorage;

        // Vi injicerar den HttpClient som (förhoppningsvis) redan skickar med JWT-token automatiskt
        public FrontendProfileService(HttpClient httpClient, ProtectedSessionStorage sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
        }

        public async Task<UserProfileDto?> GetProfileAsync()
        {
            await SetAuthorizationHeaderAsync(); // Hämta och sätt token!

            try
            {
                var response = await _httpClient.GetAsync("api/profile");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserProfileDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<AuthResponseDto> ChangePasswordAsync(ChangePasswordDto dto)
        {
            await SetAuthorizationHeaderAsync();

            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/profile/change-password", dto);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResponseDto>()
                           ?? new AuthResponseDto(false, "Ett oväntat fel uppstod.", null);
                }

                var errorResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                return errorResult ?? new AuthResponseDto(false, "Kunde inte byta lösenord.", null);
            }
            catch (Exception ex)
            {
                return new AuthResponseDto(false, $"Nätverksfel: {ex.Message}", null);
            }
        }

        public async Task<AuthResponseDto> ChangeEmailAsync(ChangeEmailDto dto)
        {
            await SetAuthorizationHeaderAsync();

            // Samma logik som för ChangePassword
            var response = await _httpClient.PutAsJsonAsync("api/profile/change-email", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>()
                       ?? new AuthResponseDto(false, "Ett oväntat fel uppstod.", null);
            }

            var errorResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return errorResult ?? new AuthResponseDto(false, "Kunde inte byta e-post.", null);
        }

        // Hjälpmetod för att hämta token och lägga i headern
        private async Task SetAuthorizationHeaderAsync()
        {
            try
            {
                var tokenResult = await _sessionStorage.GetAsync<string>("authToken"); // OBS: Se till att "authToken" är exakt det du sparade inloggningen som!
                if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
                }
            }
            catch
            {
                // Fångar upp felet om detta anropas för tidigt i prerendering-fasen
            }
        }
    }
}
