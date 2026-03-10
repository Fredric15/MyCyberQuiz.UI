using MyCyberQuiz.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCyberQuiz.BLL.Interfaces
{
    public interface IAiChatService
    {
        Task<AiChatResponseDto> GetChatResponseAsync(AiChatRequestDto request);
    }
}
