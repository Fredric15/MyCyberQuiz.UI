using System;
using System.Collections.Generic;
using System.Text;

namespace MyCyberQuiz.Shared.DTOs
{
    public class AiChatRequestDto
    {
        //Om användare ställer en egen fråga, så skickas den här till backend
        public string UserQuestion { get; set; } = string.Empty;

        //Frågan från Quizet som skickas till Ai
        public string QuestionContext { get; set; } = string.Empty;
    }
}
