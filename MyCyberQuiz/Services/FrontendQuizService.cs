using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MyCyberQuiz.Shared.DTOs;
using MyCyberQuiz.UI.Services.Interfaces;
using System.Net.Http.Headers;

namespace MyCyberQuiz.UI.Services
{
    public class FrontendQuizService : IFrontendQuizService
    {
        private readonly HttpClient _httpClient;
        private readonly ProtectedSessionStorage _sessionStorage;

        public FrontendQuizService(HttpClient httpClient, ProtectedSessionStorage sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
        }
        public async Task<List<CategoryDto>> GetQuizMenuAsync()
        {
            try
            {
                // 1. Leta fram passerkortet (Token) från minnet
                var tokenResult = await _sessionStorage.GetAsync<string>("authToken");

                if (tokenResult.Success && !string.IsNullOrWhiteSpace(tokenResult.Value))
                {
                    // 2. Sätt fast tokenen på HttpClient:en (Detta är kod-motsvarigheten till hänglåset i Swagger!)
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", tokenResult.Value);
                }

                // 3. Gör anropet till API:et
                var response = await _httpClient.GetAsync("api/Quiz/menu"); // Byt till din exakta endpoint

                if (response.IsSuccessStatusCode)
                {
                    // Läs ut datan om allt gick bra
                    return await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
                }

                // Om vi t.ex. får 401 Unauthorized (token har gått ut)
                return new List<CategoryDto>();
            }
            catch
            {
                return new List<CategoryDto>();
            }
        }

        public async Task<QuizDetailsDto?> GetQuizByIdAsync(int id)
        {

            var tokenResult = await _sessionStorage.GetAsync<string>("authToken");

            if (tokenResult.Success && !string.IsNullOrWhiteSpace(tokenResult.Value))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", tokenResult.Value);
            }

            var response = await _httpClient.GetAsync($"api/Quiz/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<QuizDetailsDto>();
                
            }

            return null;
        }

        public async Task<QuizResultDto?> SubmitQuizAsync(SubmitQuizDto submission)
        {
            var tokenResult = await _sessionStorage.GetAsync<string>("authToken");

            if (tokenResult.Success && !string.IsNullOrWhiteSpace(tokenResult.Value))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", tokenResult.Value);
            }

            var response = await _httpClient.PostAsJsonAsync("api/Quiz/submit", submission);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<QuizResultDto>();
            }

            return null;
        }

        public async Task<AnswerFeedbackDto> CheckSingleAnswerAsync(int questionId, int selectedOptionId)
        {
            // Ropa på din nya endpoint i API:et!
            var response = await _httpClient.GetFromJsonAsync<AnswerFeedbackDto>(
                $"api/quiz/question/{questionId}/check/{selectedOptionId}");

            return response ?? new AnswerFeedbackDto(false, 0, "Kunde inte hämta svar");
        }
    }
}
