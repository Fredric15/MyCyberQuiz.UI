using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MyCyberQuiz.Shared.DTOs;
using System.Net.Http.Headers;

namespace MyCyberQuiz.UI.Services.Interfaces
{
    public class FrontendAiChatService : IFrontendAiChatService
    {
        private readonly HttpClient _httpClient;
        private readonly ProtectedSessionStorage _sessionStorage;

        public FrontendAiChatService(HttpClient httpClient, ProtectedSessionStorage sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
        }

        public async Task<AiChatResponseDto> SendMessageAsync(AiChatRequestDto request)
        {
            try
            {
                // Hämta och sätt på token
                var tokenResult = await _sessionStorage.GetAsync<string>("authToken");
                if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
                }

                var response = await _httpClient.PostAsJsonAsync("api/aichat/ask", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AiChatResponseDto>()
                           ?? new AiChatResponseDto { IsSuccessful = false, ErrorMessage = "Tomt svar från servern." };
                }

                return new AiChatResponseDto { IsSuccessful = false, ErrorMessage = "Kunde inte nå AI-motorn." };
            }
            catch (Exception ex)
            {
                return new AiChatResponseDto { IsSuccessful = false, ErrorMessage = ex.Message };
            }
        }
    }
}
