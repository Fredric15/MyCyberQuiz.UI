using System;
using System.Collections.Generic;
using System.Text;

namespace MyCyberQuiz.Shared.DTOs
{
    public class AiChatResponseDto
    {
        public string Reply { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; } = true;
        public string ErrorMessage { get; set; }
    }
}
