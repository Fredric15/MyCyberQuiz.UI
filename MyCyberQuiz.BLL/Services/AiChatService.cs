using Microsoft.Extensions.Configuration;
using MyCyberQuiz.BLL.Interfaces;
using MyCyberQuiz.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MyCyberQuiz.BLL.Services
{

    public class AiChatService : IAiChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _ollamaModel;

        public AiChatService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;

            // Vi sätter BaseAddress precis som i din gamla kod!
            var baseUrl = config["OllamaSettings:BaseUrl"] ?? "http://localhost:11434/";
            _httpClient.BaseAddress = new Uri(baseUrl);

            // Hämtar modellnamnet från appsettings.json (t.ex. "phi3" eller "llama3")
            _ollamaModel = config["OllamaSettings:ModelName"] ?? "phi3";
        }

        public async Task<AiChatResponseDto> GetChatResponseAsync(AiChatRequestDto request)
        {
            try
            {
                // 1. Vi bygger ihop en "prompt" med kontexten precis som uppgiften krävde
                string fullPrompt = $@"Du är en hjälpsam lärare i cybersäkerhet. Svara kort och pedagogiskt på svenska.
                    Kontext från quizet: {request.QuestionContext}
                    Användarens fråga: {request.UserQuestion}";

                // 2. Vi bygger request-objektet precis som ni gjorde i övningen
                var ollamaRequest = new
                {
                    model = _ollamaModel, // Flexibelt modellnamn!
                    prompt = fullPrompt,
                    stream = false
                };

                // 3. Skicka till Ollama
                var response = await _httpClient.PostAsJsonAsync("/api/generate", ollamaRequest);

                if (response.IsSuccessStatusCode)
                {
                    // 4. Använder OllamaResponse-klassen för att läsa av svaret
                    var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();

                    return new AiChatResponseDto
                    {
                        IsSuccessful = true,
                        Reply = result?.response ?? "Fick inget svar från AI:n."
                    };
                }

                return new AiChatResponseDto { IsSuccessful = false, ErrorMessage = "Kunde inte nå Ollama." };
            }
            catch (Exception ex)
            {
                return new AiChatResponseDto { IsSuccessful = false, ErrorMessage = $"Ett fel uppstod: {ex.Message}" };
            }
        }

        // Här är er klass från övningen, smidigt inbakad här nere!
        private class OllamaResponse
        {
            public string response { get; set; } = string.Empty;
        }


    }
}