using System;
using System.Collections.Generic;
using System.Text;

namespace MyCyberQuiz.Shared.DTOs
{
    // Record nyare variant av en klass, som är mer optimerad för immutability och enklare syntax

    public record QuizDetailsDto(int Id, string Title, string Description, List<QuestionDto> Questions);


    public record QuestionDto(int Id, string Text, List<OptionDto> Options);

    // Vi skickar INTE med IsCorrect till klienten för att förhindra fusk
    public record OptionDto(int Id, string Text);

    // Detta är det lilla "kuvertet" API:et skickar tillbaka
    public record AnswerFeedbackDto(
        bool IsCorrect,
        int CorrectOptionId,
        string CorrectAnswerText
    );

}
