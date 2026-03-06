using MyCyberQuiz.Shared.DTOs;

namespace MyCyberQuiz.UI.Services.Interfaces
{
    public interface IFrontendAiChatService
    {
        Task<AiChatResponseDto> SendMessageAsync(AiChatRequestDto request);
    }
}
